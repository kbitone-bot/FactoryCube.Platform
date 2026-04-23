"""Synthetic Data Validation Engine"""
import json
import numpy as np
import pandas as pd
from pathlib import Path
from typing import Dict, List


class SyntheticValidator:
    """원본 vs 합성 데이터 검증 엔진"""

    def __init__(self, config: dict):
        self.config = config
        self.thresholds = config.get("thresholds", {
            "mean_diff_pct": 15.0,
            "std_diff_pct": 25.0,
            "corr_diff": 0.3,
            "state_ratio_diff": 10.0,
        })

    def validate(self, original_path: str, synthetic_path: str) -> Dict:
        orig = pd.read_csv(original_path) if original_path else None
        synth = pd.read_csv(synthetic_path)
        report = {
            "overall_pass": True,
            "metrics": [],
            "details": {}
        }

        numeric_cols = synth.select_dtypes(include=[np.number]).columns.tolist()
        numeric_cols = [c for c in numeric_cols if c not in ("anomaly_label",)]

        if orig is not None:
            # 분포 비교
            for col in numeric_cols:
                if col not in orig.columns:
                    continue
                o_mean = orig[col].mean()
                s_mean = synth[col].mean()
                o_std = orig[col].std()
                s_std = synth[col].std()

                mean_diff = abs(o_mean - s_mean) / (abs(o_mean) + 1e-9) * 100
                std_diff = abs(o_std - s_std) / (abs(o_std) + 1e-9) * 100

                passed_mean = mean_diff <= self.thresholds.get("mean_diff_pct", 15.0)
                passed_std = std_diff <= self.thresholds.get("std_diff_pct", 25.0)

                report["metrics"].append({
                    "metric_name": f"{col}_mean",
                    "original_value": float(o_mean),
                    "synthetic_value": float(s_mean),
                    "difference_pct": float(mean_diff),
                    "is_passed": bool(passed_mean),
                })
                report["metrics"].append({
                    "metric_name": f"{col}_std",
                    "original_value": float(o_std),
                    "synthetic_value": float(s_std),
                    "difference_pct": float(std_diff),
                    "is_passed": bool(passed_std),
                })
                if not (passed_mean and passed_std):
                    report["overall_pass"] = False

            # 상태 비율 비교
            if "state" in orig.columns and "state" in synth.columns:
                o_states = orig["state"].value_counts(normalize=True) * 100
                s_states = synth["state"].value_counts(normalize=True) * 100
                for st in set(o_states.index) | set(s_states.index):
                    o_p = o_states.get(st, 0)
                    s_p = s_states.get(st, 0)
                    diff = abs(o_p - s_p)
                    passed = diff <= self.thresholds.get("state_ratio_diff", 10.0)
                    report["metrics"].append({
                        "metric_name": f"state_ratio_{st}",
                        "original_value": float(o_p),
                        "synthetic_value": float(s_p),
                        "difference_pct": float(diff),
                        "is_passed": bool(passed),
                    })
                    if not passed:
                        report["overall_pass"] = False

        # 도메인 룰 위배 검사
        domain_violations = self._check_domain_rules(synth)
        report["details"]["domain_violations"] = domain_violations
        if domain_violations["count"] > len(synth) * 0.01:
            report["overall_pass"] = False

        return report

    def _check_domain_rules(self, df: pd.DataFrame) -> Dict:
        violations = []
        # part_counter 감소
        if "part_counter" in df.columns:
            dec = (df["part_counter"].diff() < 0).sum()
            if dec > 0:
                violations.append({"rule": "part_counter_decrease", "count": int(dec)})
        # power_on_time 감소
        if "power_on_time" in df.columns:
            dec = (df["power_on_time"].diff() < 0).sum()
            if dec > 0:
                violations.append({"rule": "power_on_time_decrease", "count": int(dec)})
        # spindle_speed > 0 and spindle_load == 0 (공회전)
        if "spindle_speed" in df.columns and "spindle_load" in df.columns:
            air_cut = ((df["spindle_speed"] > 0) & (df["spindle_load"] == 0)).sum()
            violations.append({"rule": "air_cut_candidate", "count": int(air_cut)})
        # 온도 범위 초과
        if "temperature" in df.columns:
            over = (df["temperature"] > 80).sum()
            if over > 0:
                violations.append({"rule": "temperature_over_range", "count": int(over)})

        return {"count": sum(v["count"] for v in violations), "items": violations}
