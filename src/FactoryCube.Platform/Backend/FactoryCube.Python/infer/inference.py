"""Inference Engine"""
import json
import pickle
from pathlib import Path

import numpy as np
import pandas as pd

from features.engineering import FeatureEngineer


class InferenceEngine:
    """학습된 모델로 추론 수행"""

    def __init__(self, config: dict):
        self.config = config
        self.model_path = config.get("model_path", "")
        self.scaler_path = config.get("scaler_path", "")
        self.target = config.get("target_column", "anomaly_label")

    def run(self, input_path: str, output_dir: Path) -> dict:
        df = pd.read_csv(input_path)
        fe = FeatureEngineer(self.config.get("feature_config", {}))
        df = fe.transform(df)

        drop_cols = ["equipment_id", "ts", "state", "scenario", "failure_risk_level", "health_grade"]
        drop_cols = [c for c in drop_cols if c in df.columns]
        X = df.drop(columns=drop_cols + [self.target])
        X = X.select_dtypes(include=[np.number])

        with open(self.model_path, "rb") as f:
            model = pickle.load(f)
        with open(self.scaler_path, "rb") as f:
            scaler = pickle.load(f)

        X_s = scaler.transform(X)

        if hasattr(model, "predict_proba"):
            y_pred = model.predict(X_s)
            y_proba = model.predict_proba(X_s)[:, 1]
        elif hasattr(model, "decision_function"):
            y_pred = model.predict(X_s)
            y_proba = None
        else:
            y_pred = model.predict(X_s)
            y_proba = None

        df["predicted_class"] = y_pred
        if y_proba is not None:
            df["probability"] = y_proba

        output_dir = Path(output_dir)
        output_dir.mkdir(parents=True, exist_ok=True)
        out_csv = output_dir / "predictions.csv"
        df.to_csv(out_csv, index=False)

        return {"output_path": str(out_csv), "rows": len(df)}
