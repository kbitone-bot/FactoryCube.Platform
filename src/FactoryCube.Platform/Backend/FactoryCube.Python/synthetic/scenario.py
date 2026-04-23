"""Scenario Library and State Transition Engine"""
import numpy as np
import pandas as pd
from typing import List, Dict


class ScenarioLibrary:
    """시나리오 라이브러리"""

    SCENARIOS = [
        "normal",
        "idle",
        "air_cut",
        "heavy_load",
        "memory_leak",
        "drift",
        "spike",
        "stuck",
        "dropout",
        "restart_burst",
        "hung",
        "alarm_burst",
    ]

    def __init__(self, rng: np.random.Generator):
        self.rng = rng

    def pick_scenario(self, state: str, weights: Dict[str, float] = None) -> str:
        if weights is None:
            weights = {s: 1.0 for s in self.SCENARIOS}
        candidates = list(weights.keys())
        probs = np.array([weights.get(c, 1.0) for c in candidates])
        probs = probs / probs.sum()
        return self.rng.choice(candidates, p=probs)


class StateTransitionEngine:
    """상태 전이 엔진"""

    VALID_TRANSITIONS = {
        "OFF": ["READY"],
        "READY": ["RUNNING", "IDLE", "MAINTENANCE"],
        "RUNNING": ["IDLE", "ALARM", "HANG", "ERROR", "READY"],
        "IDLE": ["RUNNING", "READY", "OFF"],
        "HANG": ["ERROR", "READY", "RUNNING"],
        "ALARM": ["READY", "RUNNING", "MAINTENANCE"],
        "ERROR": ["READY", "OFF", "MAINTENANCE"],
        "MAINTENANCE": ["READY", "OFF"],
    }

    def __init__(self, rng: np.random.Generator):
        self.rng = rng

    def generate_transitions(
        self,
        start: pd.Timestamp,
        end: pd.Timestamp,
        scenarios: List[str] = None,
    ) -> List[Dict]:
        transitions = []
        current_time = start
        state = "OFF"

        while current_time < end:
            duration = self._duration_for_state(state)
            scenario = self._pick_scenario_for_state(state, scenarios)
            transitions.append({
                "state": state,
                "start": current_time,
                "duration_sec": min(duration, int((end - current_time).total_seconds())),
                "scenario": scenario,
            })
            current_time += pd.Timedelta(seconds=duration)

            # 상태 전이
            next_candidates = self.VALID_TRANSITIONS.get(state, [state])
            if state == "RUNNING" and self.rng.random() < 0.1:
                next_state = self.rng.choice(["ALARM", "HANG", "ERROR"])
            elif state == "HANG" and self.rng.random() < 0.3:
                next_state = "ERROR"
            elif state == "ERROR" and self.rng.random() < 0.5:
                next_state = "READY"
            else:
                next_state = self.rng.choice(next_candidates)
            state = next_state

        return transitions

    def _duration_for_state(self, state: str) -> int:
        """상태별 지속 시간(초)"""
        ranges = {
            "OFF": (300, 1800),
            "READY": (60, 300),
            "RUNNING": (600, 7200),
            "IDLE": (120, 1800),
            "HANG": (60, 600),
            "ALARM": (30, 300),
            "ERROR": (60, 600),
            "MAINTENANCE": (300, 3600),
        }
        lo, hi = ranges.get(state, (60, 300))
        return self.rng.integers(lo, hi)

    def _pick_scenario_for_state(self, state: str, scenarios: List[str]) -> str:
        if state in ("OFF", "READY"):
            return "normal"
        if state == "IDLE":
            return "idle"
        if state == "HANG":
            return "hung"
        if state == "ALARM":
            return "alarm_burst"
        if state == "ERROR":
            return "restart_burst"
        if scenarios:
            return self.rng.choice(scenarios)
        return "normal"
