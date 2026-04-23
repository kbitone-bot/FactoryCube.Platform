"""Synthetic Data Generator for PLC and Test Equipment"""
import numpy as np
import pandas as pd
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import json

from synthetic.scenario import ScenarioLibrary, StateTransitionEngine


class SyntheticGenerator:
    """PLC 및 시험장비 시계열 합성데이터 생성기"""

    def __init__(self, config: dict):
        self.config = config
        self.seed = config.get("seed", 42)
        self.rng = np.random.default_rng(self.seed)
        self.scenario_lib = ScenarioLibrary(self.rng)
        self.state_engine = StateTransitionEngine(self.rng)
        self.noise_level = float(config.get("noise_level", 0.02))
        self.drift_level = float(config.get("drift_level", 0.0))
        self.anomaly_ratio = float(config.get("anomaly_ratio", 0.05))
        self.scenarios = config.get("scenario_config", {}).get("scenarios", ["normal"])
        self.equipment_count = int(config.get("equipment_count", 1))
        self.start_time = pd.to_datetime(config.get("start_time", "2024-01-01 00:00:00"))
        self.end_time = pd.to_datetime(config.get("end_time", "2024-01-02 00:00:00"))

    def generate(self) -> pd.DataFrame:
        records = []
        for eq_idx in range(self.equipment_count):
            eq_id = f"EQ{eq_idx + 1:03d}"
            eq_records = self._generate_equipment(eq_id)
            records.extend(eq_records)
        df = pd.DataFrame(records)
        df = self._inject_anomalies(df)
        df = self._generate_labels(df)
        return df

    def _generate_equipment(self, equipment_id: str) -> List[dict]:
        records = []
        current_time = self.start_time
        state = "OFF"
        state_end = None
        baseline_cycle = 45.0
        part_counter = 0
        power_on_time = 0.0
        run_time = 0.0
        cut_time = 0.0
        mem_base = 120.0
        mem_leak_rate = 0.0
        drift_offset = 0.0
        spindle_baseline = 3000.0

        # 상태 전이 시퀀스 생성
        transitions = self.state_engine.generate_transitions(
            self.start_time, self.end_time, scenarios=self.scenarios
        )

        trans_idx = 0
        while current_time < self.end_time and trans_idx < len(transitions):
            trans = transitions[trans_idx]
            state = trans["state"]
            duration = trans["duration_sec"]
            scenario = trans.get("scenario", "normal")

            # 시나리오별 파라미터
            if scenario == "idle":
                spindle_baseline = 0.0
                mem_leak_rate = 0.0
            elif scenario == "air_cut":
                spindle_baseline = 2500.0
            elif scenario == "heavy_load":
                spindle_baseline = 4500.0
            elif scenario == "memory_leak":
                mem_leak_rate = 0.05
            elif scenario == "drift":
                drift_offset += self.rng.normal(0.001, 0.0005)
            else:
                spindle_baseline = 3000.0
                mem_leak_rate = 0.0

            t_end = min(current_time + pd.Timedelta(seconds=duration), self.end_time)
            while current_time < t_end:
                dt = 1.0  # 1초 간격
                in_cycle = state == "RUNNING"

                # Spindle speed
                if state in ("OFF", "READY", "HANG"):
                    spindle_speed = 0.0
                else:
                    spindle_speed = spindle_baseline + self._periodic(current_time) + self._noise() * 50
                    spindle_speed = max(0, spindle_speed)

                # Feed rate
                feed_rate = 800.0 if in_cycle else 0.0
                feed_rate += self._noise() * 20

                # Spindle load
                if spindle_speed > 0 and in_cycle:
                    spindle_load = 25.0 + (spindle_speed / 3000.0) * 30.0 + self._noise() * 5
                elif spindle_speed > 0 and not in_cycle:
                    spindle_load = 5.0 + self._noise() * 2  # air cut candidate
                else:
                    spindle_load = 0.0

                # Servo loads
                servo_loads = [0.0] * 5
                if in_cycle:
                    for i in range(5):
                        servo_loads[i] = 10.0 + self.rng.random() * 20.0 + self._noise() * 3

                # Cycle time
                if in_cycle:
                    cycle_time = baseline_cycle + self._noise() * 2
                    run_time += dt
                    cut_time += dt * 0.7
                else:
                    cycle_time = 0.0

                # Part counter
                if in_cycle and self.rng.random() < (1.0 / baseline_cycle):
                    part_counter += 1

                power_on_time += dt

                # Process metrics (test equipment common)
                proc_cpu = self.rng.random() * 5.0 if not in_cycle else 15.0 + self.rng.random() * 20.0
                proc_mem = mem_base + mem_leak_rate * (current_time - self.start_time).total_seconds()
                proc_mem += self._noise() * 2

                # Host metrics
                host_cpu = proc_cpu + self.rng.random() * 10.0
                host_mem_pct = 45.0 + self._noise() * 5.0
                host_uptime = (current_time - self.start_time).total_seconds()

                # Test equipment UI measurements
                pressure = 1.0 + self._noise() * 0.05 + drift_offset if state != "OFF" else 0.0
                temperature = 25.0 + (spindle_load / 100.0) * 15.0 + self._noise() * 0.5
                air_pressure = 0.6 + self._noise() * 0.02

                # Positions
                abs_pos = [self.rng.random() * 500 for _ in range(5)] if in_cycle else [0.0] * 5
                dist_togo = [self.rng.random() * 10 for _ in range(5)] if in_cycle else [0.0] * 5

                record = {
                    "equipment_id": equipment_id,
                    "ts": current_time,
                    "state": state,
                    "scenario": scenario,
                    "spindle_speed": round(spindle_speed, 2),
                    "feed_rate": round(feed_rate, 2),
                    "spindle_load": round(spindle_load, 2),
                    "servo_load_1": round(servo_loads[0], 2),
                    "servo_load_2": round(servo_loads[1], 2),
                    "servo_load_3": round(servo_loads[2], 2),
                    "servo_load_4": round(servo_loads[3], 2),
                    "servo_load_5": round(servo_loads[4], 2),
                    "cycle_time_sec": round(cycle_time, 2),
                    "part_counter": part_counter,
                    "power_on_time": round(power_on_time, 1),
                    "run_time_sec": round(run_time, 1),
                    "cut_time_sec": round(cut_time, 1),
                    "proc_cpu_pct": round(proc_cpu, 2),
                    "proc_mem_ws_mb": round(proc_mem, 2),
                    "host_cpu_pct": round(host_cpu, 2),
                    "host_mem_pct": round(host_mem_pct, 2),
                    "host_uptime_sec": round(host_uptime, 1),
                    "pressure": round(pressure, 4),
                    "temperature": round(temperature, 2),
                    "air_pressure": round(air_pressure, 4),
                    "abs_pos_1": round(abs_pos[0], 3),
                    "abs_pos_2": round(abs_pos[1], 3),
                    "abs_pos_3": round(abs_pos[2], 3),
                    "abs_pos_4": round(abs_pos[3], 3),
                    "abs_pos_5": round(abs_pos[4], 3),
                    "dist_togo_1": round(dist_togo[0], 3),
                    "dist_togo_2": round(dist_togo[1], 3),
                    "dist_togo_3": round(dist_togo[2], 3),
                    "dist_togo_4": round(dist_togo[3], 3),
                    "dist_togo_5": round(dist_togo[4], 3),
                }
                records.append(record)
                current_time += pd.Timedelta(seconds=dt)
            trans_idx += 1
        return records

    def _periodic(self, t: pd.Timestamp) -> float:
        sec = t.second + t.microsecond / 1e6
        return 50.0 * np.sin(2 * np.pi * sec / 60.0)

    def _noise(self) -> float:
        return self.rng.normal(0, self.noise_level)

    def _inject_anomalies(self, df: pd.DataFrame) -> pd.DataFrame:
        if self.anomaly_ratio <= 0:
            return df
        n = len(df)
        anomaly_count = int(n * self.anomaly_ratio)
        idx = self.rng.choice(n, size=anomaly_count, replace=False)

        for i in idx:
            anomaly_type = self.rng.choice(["spike", "stuck", "dropout", "drift_spike"])
            if anomaly_type == "spike":
                col = self.rng.choice(["spindle_load", "spindle_speed", "proc_cpu_pct", "temperature"])
                df.loc[i, col] = float(df.loc[i, col]) * (2.0 + self.rng.random() * 2.0)
            elif anomaly_type == "stuck":
                if i > 0:
                    for c in ["spindle_speed", "spindle_load", "pressure"]:
                        df.loc[i, c] = df.loc[i - 1, c]
            elif anomaly_type == "dropout":
                for c in ["spindle_speed", "spindle_load", "feed_rate"]:
                    df.loc[i, c] = 0.0
            elif anomaly_type == "drift_spike":
                df.loc[i, "pressure"] = float(df.loc[i, "pressure"]) + 0.5 * self.rng.random()
        return df

    def _generate_labels(self, df: pd.DataFrame) -> pd.DataFrame:
        # 도메인 룰 기반 라벨
        df["anomaly_label"] = 0
        df["failure_risk_level"] = "LOW"
        df["health_grade"] = "A"

        # 공회전 후보: spindle_speed > 0 and spindle_load < 5
        air_cut_mask = (df["spindle_speed"] > 0) & (df["spindle_load"] < 5)
        df.loc[air_cut_mask, "anomaly_label"] = 1

        # 부하 급증
        overload_mask = df["spindle_load"] > 80
        df.loc[overload_mask, "anomaly_label"] = 1
        df.loc[overload_mask, "failure_risk_level"] = "HIGH"
        df.loc[overload_mask, "health_grade"] = "D"

        # 온도 이상
        temp_mask = df["temperature"] > 45
        df.loc[temp_mask, "anomaly_label"] = 1
        df.loc[temp_mask, "failure_risk_level"] = "MEDIUM"
        df.loc[temp_mask & (df["failure_risk_level"] != "HIGH"), "health_grade"] = "C"

        # 메모리 누수 후보
        mem_mask = df["proc_mem_ws_mb"] > 400
        df.loc[mem_mask, "anomaly_label"] = 1
        df.loc[mem_mask, "failure_risk_level"] = "MEDIUM"

        return df
