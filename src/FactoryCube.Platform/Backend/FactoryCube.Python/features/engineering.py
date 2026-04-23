"""Feature Engineering Module"""
import numpy as np
import pandas as pd
from typing import List


class FeatureEngineer:
    """PLC 및 시험장비 공통 Feature Engineering"""

    def __init__(self, config: dict = None):
        self.config = config or {}
        self.lag_seconds = self.config.get("lag_seconds", [1, 5, 10, 30, 60])
        self.rolling_windows = self.config.get("rolling_windows", [5, 10, 30, 60])

    def transform(self, df: pd.DataFrame) -> pd.DataFrame:
        df = df.copy()
        df = df.sort_values(["equipment_id", "ts"]).reset_index(drop=True)

        numeric_cols = df.select_dtypes(include=[np.number]).columns.tolist()
        exclude = ["anomaly_label", "part_counter"]
        numeric_cols = [c for c in numeric_cols if c not in exclude]

        # Lag features
        for col in numeric_cols:
            for lag in self.lag_seconds:
                df[f"{col}_lag_{lag}"] = df.groupby("equipment_id")[col].shift(lag)

        # Rolling statistics
        for col in numeric_cols:
            for w in self.rolling_windows:
                df[f"{col}_roll_mean_{w}"] = (
                    df.groupby("equipment_id")[col]
                    .rolling(w, min_periods=1).mean()
                    .reset_index(level=0, drop=True)
                )
                df[f"{col}_roll_std_{w}"] = (
                    df.groupby("equipment_id")[col]
                    .rolling(w, min_periods=1).std()
                    .reset_index(level=0, drop=True)
                )

        # 도메인 파생 Feature
        df["cycle_time_delta"] = df.groupby("equipment_id")["cycle_time_sec"].diff()
        df["memory_growth_rate"] = df.groupby("equipment_id")["proc_mem_ws_mb"].diff()
        df["cpu_spike_flag"] = (df["proc_cpu_pct"] > df["proc_cpu_pct"].quantile(0.95)).astype(int)
        df["flatline_flag"] = (
            df.groupby("equipment_id")["spindle_speed"]
            .rolling(10, min_periods=5).apply(lambda x: x.max() - x.min() < 0.01, raw=True)
            .reset_index(level=0, drop=True)
            .fillna(0)
        )
        df["is_air_cut_candidate"] = ((df["spindle_speed"] > 0) & (df["spindle_load"] < 5)).astype(int)
        df["drift_score"] = (
            df.groupby("equipment_id")["pressure"]
            .rolling(60, min_periods=10).apply(lambda x: np.polyfit(range(len(x)), x, 1)[0], raw=True)
            .reset_index(level=0, drop=True)
        )
        df["anomaly_score_rule_based"] = (
            df["is_air_cut_candidate"] * 0.3
            + df["cpu_spike_flag"] * 0.3
            + df["flatline_flag"] * 0.2
            + (df["temperature"] > 45).astype(int) * 0.2
        )

        # 결측치 처리
        df = df.fillna(0)
        return df
