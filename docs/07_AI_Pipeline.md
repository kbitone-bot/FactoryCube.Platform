# AI 분석 파이프라인 설계

## 1. 학습 파이프라인

```
[Dataset] → [Feature Engineering] → [Train/Val Split] → [Scaling/Encoding]
    → [Baseline 학습] → [모델 비교] → [Metric 저장] → [Model Registry]
```

### 1.1 Dataset Split
- 시계열 데이터이므로 random split 금지
- Time-aware split: 과거 80% → 학습, 최근 20% → 검증
- Equipment-aware split: 동일 장비 내에서 시간 기준 분할

### 1.2 Feature Engineering
- Lag features: 1s, 5s, 10s, 30s, 60s
- Rolling statistics: mean, std, max, min (5, 10, 30, 60 window)
- 도메인 파생: cycle_time_delta, memory_growth_rate, drift_score
- 상태 인코딩: one-hot 또는 ordinal

### 1.3 Scaling/Encoding
- Numeric: StandardScaler (z-score)
- Categorical: OneHotEncoder 또는 LabelEncoder
- 시간: cyclical encoding (sin/cos of hour, dayofweek)

## 2. 모델 카탈로그

| 과제 | 기본 모델 | 후보 모델 |
|------|-----------|-----------|
| 장비 상태 분류 | XGBoost | LightGBM, RandomForest |
| 가동/비가동 판정 | LogisticRegression | XGBoost, Rule Engine |
| 이상 탐지 | IsolationForest | OneClassSVM, Autoencoder |
| 알람 예측 | XGBoost + TimeSeries | ARIMA + Gradient Boosting |
| cycle time 예측 | XGBoost Regressor | LightGBM, RandomForest |
| spindle load 예측 | XGBoost Regressor | LightGBM |
| maintenance priority | XGBoost Classifier | Rule + ML ensemble |

## 3. 평가 지표

### 분류
- accuracy, precision, recall, f1
- roc_auc (이진/다중 클래스)

### 회귀
- mae, rmse, mape, r2

### 이상탐지
- precision@k, recall@k
- pr_auc
- 탐지 구간 적중률 (예측 시점 ± 30초 내 실제 이상)

## 4. Explainability

### Feature Importance
- Tree-based: 기본 feature_importances_
- Permutation importance: 검증 세트 기반

### SHAP (2차 고도화)
- summary plot
- waterfall plot (개별 예측 해석)

## 5. 모델 레지스트리

```json
{
  "experimentId": "uuid",
  "modelVersion": 3,
  "modelPath": "artifacts/model_v3.pkl",
  "metrics": {
    "f1": 0.92,
    "precision": 0.89,
    "recall": 0.95
  },
  "featureImportance": { "spindle_load_roll_mean_10": 0.15, ... },
  "isDeployed": true,
  "deployedAt": "2024-01-15T00:00:00Z"
}
```

## 6. 추론 파이프라인

```
[실시간/배치 데이터] → [Feature Engineering] → [Scaler 적용]
    → [Model.predict] → [Post-processing] → [PredictionResult 저장]
    → [Dashboard Snapshot 갱신]
```
