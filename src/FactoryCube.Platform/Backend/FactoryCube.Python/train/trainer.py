"""ML Training Module"""
import json
import pickle
from pathlib import Path
from typing import Dict

import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split, TimeSeriesSplit
from sklearn.preprocessing import StandardScaler, LabelEncoder
from sklearn.ensemble import RandomForestClassifier, IsolationForest
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import (
    accuracy_score, precision_score, recall_score, f1_score, roc_auc_score,
    mean_absolute_error, mean_squared_error, r2_score,
)
import xgboost as xgb

from features.engineering import FeatureEngineer


class MlTrainer:
    """모델 학습기"""

    MODEL_CATALOG = {
        "classification": ["RandomForest", "XGBoost", "LogisticRegression"],
        "regression": ["XGBoost", "RandomForest"],
        "anomaly_detection": ["IsolationForest"],
        "time_series": ["XGBoost"],
    }

    def __init__(self, config: dict):
        self.config = config
        self.task_type = config.get("task_type", "classification")
        self.model_type = config.get("model_type", "XGBoost")
        self.target = config.get("target_column", "anomaly_label")
        self.test_size = config.get("test_size", 0.2)
        self.seed = config.get("seed", 42)

    def train(self, input_path: str, output_dir: Path) -> Dict:
        df = pd.read_csv(input_path)
        fe = FeatureEngineer(self.config.get("feature_config", {}))
        df = fe.transform(df)

        # Feature / Target 분리
        drop_cols = ["equipment_id", "ts", "state", "scenario", "failure_risk_level", "health_grade"]
        drop_cols = [c for c in drop_cols if c in df.columns]
        X = df.drop(columns=drop_cols + [self.target])
        y = df[self.target]

        # 문자열 컬럼 제거
        X = X.select_dtypes(include=[np.number])

        # Time-aware split (equipment_id 기준)
        train_idx, test_idx = self._time_split(df)
        X_train, X_test = X.iloc[train_idx], X.iloc[test_idx]
        y_train, y_test = y.iloc[train_idx], y.iloc[test_idx]

        scaler = StandardScaler()
        X_train_s = scaler.fit_transform(X_train)
        X_test_s = scaler.transform(X_test)

        model = self._build_model()
        if self.model_type == "IsolationForest":
            model.fit(X_train_s)
            y_pred = model.predict(X_test_s)
            y_pred = np.where(y_pred == -1, 1, 0)
            metrics = {
                "precision": float(precision_score(y_test, y_pred, zero_division=0)),
                "recall": float(recall_score(y_test, y_pred, zero_division=0)),
                "f1": float(f1_score(y_test, y_pred, zero_division=0)),
            }
        elif self.task_type == "classification":
            model.fit(X_train_s, y_train)
            y_pred = model.predict(X_test_s)
            y_proba = model.predict_proba(X_test_s)[:, 1] if hasattr(model, "predict_proba") else None
            metrics = {
                "accuracy": float(accuracy_score(y_test, y_pred)),
                "precision": float(precision_score(y_test, y_pred, zero_division=0)),
                "recall": float(recall_score(y_test, y_pred, zero_division=0)),
                "f1": float(f1_score(y_test, y_pred, zero_division=0)),
            }
            if y_proba is not None:
                metrics["roc_auc"] = float(roc_auc_score(y_test, y_proba))
        else:
            model.fit(X_train_s, y_train)
            y_pred = model.predict(X_test_s)
            metrics = {
                "mae": float(mean_absolute_error(y_test, y_pred)),
                "rmse": float(np.sqrt(mean_squared_error(y_test, y_pred))),
                "r2": float(r2_score(y_test, y_pred)),
            }

        # Feature importance
        importance = {}
        if hasattr(model, "feature_importances_"):
            importance = dict(zip(X.columns, model.feature_importances_.tolist()))
        elif hasattr(model, "coef_"):
            importance = dict(zip(X.columns, np.abs(model.coef_[0]).tolist()))

        # Save artifacts
        output_dir = Path(output_dir)
        output_dir.mkdir(parents=True, exist_ok=True)
        with open(output_dir / "model.pkl", "wb") as f:
            pickle.dump(model, f)
        with open(output_dir / "scaler.pkl", "wb") as f:
            pickle.dump(scaler, f)
        with open(output_dir / "metrics.json", "w", encoding="utf-8") as f:
            json.dump(metrics, f, ensure_ascii=False, indent=2)
        with open(output_dir / "importance.json", "w", encoding="utf-8") as f:
            json.dump(importance, f, ensure_ascii=False, indent=2)

        return {"metrics": metrics, "model_path": str(output_dir / "model.pkl")}

    def _time_split(self, df: pd.DataFrame):
        # equipment_id별로 시간순 정렬 후 분할
        df = df.sort_values(["equipment_id", "ts"]).reset_index(drop=True)
        n = len(df)
        split_idx = int(n * (1 - self.test_size))
        return list(range(split_idx)), list(range(split_idx, n))

    def _build_model(self):
        if self.model_type == "RandomForest":
            return RandomForestClassifier(n_estimators=200, random_state=self.seed, n_jobs=-1)
        if self.model_type == "XGBoost":
            return xgb.XGBClassifier(
                n_estimators=200, max_depth=6, learning_rate=0.1,
                subsample=0.8, colsample_bytree=0.8,
                random_state=self.seed, use_label_encoder=False, eval_metric="logloss"
            )
        if self.model_type == "LogisticRegression":
            return LogisticRegression(max_iter=1000, random_state=self.seed)
        if self.model_type == "IsolationForest":
            return IsolationForest(n_estimators=200, contamination=0.05, random_state=self.seed)
        raise ValueError(f"Unsupported model type: {self.model_type}")
