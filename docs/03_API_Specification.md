# FactoryCube.Platform - API 명세

## Base URL
```
http://localhost:5000/api
```

## 1. 프로젝트 관리

### GET /projects
프로젝트 목록 조회

**Response**
```json
[
  {
    "id": "uuid",
    "name": "string",
    "description": "string",
    "equipmentType": "PLC|TestEquipment|Hybrid",
    "status": "Active|Archived|Deleted",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z",
    "createdBy": "string"
  }
]
```

### GET /projects/{id}
프로젝트 상세 조회

### POST /projects
프로젝트 생성

**Request Body**
```json
{
  "name": "PLC 생산라인 A",
  "description": "FANUC 계열 PLC 데이터",
  "equipmentType": "PLC"
}
```

### PUT /projects/{id}
프로젝트 수정

### DELETE /projects/{id}
프로젝트 삭제 (soft delete)

---

## 2. 데이터셋 관리

### GET /datasets?projectId={projectId}
프로젝트별 데이터셋 목록

### GET /datasets/{id}
데이터셋 상세 조회

### POST /datasets
데이터셋 생성

**Request Body**
```json
{
  "projectId": "uuid",
  "name": "2024-01 PLC 원천데이터",
  "description": "string",
  "sourceType": "Upload|Watcher|Api|Synthetic"
}
```

### POST /datasets/{id}/upload
파일 업로드 (multipart/form-data)

### POST /datasets/{id}/ingest
원천 데이터 적재 실행 (Background Job 트리거)

---

## 3. 합성데이터

### GET /synthetic/projects/{projectId}/jobs
합성데이터 생성 Job 목록

### GET /synthetic/jobs/{id}
Job 상세 조회

### POST /synthetic/jobs
합성데이터 Job 생성

**Request Body**
```json
{
  "projectId": "uuid",
  "jobName": "정상+이상 시나리오",
  "startTime": "2024-01-01T00:00:00Z",
  "endTime": "2024-01-02T00:00:00Z",
  "equipmentCount": 5,
  "scenarioConfig": {
    "scenarios": ["normal", "heavy_load", "memory_leak"],
    "noiseLevel": 0.02,
    "driftLevel": 0.001,
    "seed": 42,
    "anomalyRatio": 0.05
  }
}
```

### GET /synthetic/validations/{syntheticDatasetId}
합성데이터 검증 결과 조회

---

## 4. 품질 검증

### GET /quality/rules?projectId={projectId}
품질 규칙 목록

### POST /quality/rules
품질 규칙 등록

**Request Body**
```json
{
  "projectId": "uuid",
  "ruleName": "spindle_load 음수 검사",
  "ruleType": "Value",
  "targetFields": ["spindle_load"],
  "conditionExpr": "value < 0",
  "severity": "Critical"
}
```

### POST /quality/datasets/{datasetId}/check
품질 검증 실행

### GET /quality/datasets/{datasetId}/latest
최신 품질 검증 결과

---

## 5. AI/ML 실험

### GET /ml/projects/{projectId}/experiments
실험 목록

### GET /ml/experiments/{id}
실험 상세

### POST /ml/experiments
실험 생성

**Request Body**
```json
{
  "projectId": "uuid",
  "experimentName": "이상탐지_v1",
  "taskType": "CLASSIFICATION|REGRESSION|ANOMALY_DETECTION|TIME_SERIES",
  "datasetId": "uuid",
  "modelType": "XGBoost|RandomForest|IsolationForest|LightGBM|LogisticRegression",
  "featureConfig": { "lag_seconds": [1,5,10], "rolling_windows": [5,10,30] },
  "hyperparameters": { "n_estimators": 200, "max_depth": 6 },
  "trainConfig": { "test_size": 0.2, "seed": 42 }
}
```

### POST /ml/experiments/{id}/train
학습 시작

### POST /ml/experiments/{id}/infer?datasetId={datasetId}
추론 실행

### GET /ml/experiments/{id}/predictions
예측 결과 조회

---

## 6. 대시보드

### GET /dashboard/projects/{projectId}/snapshot
대시보드 스냅샷

### GET /dashboard/projects/{projectId}/equipment-status
장비 상태 요약

**Response**
```json
[
  {
    "equipmentId": "EQ001",
    "currentState": "RUNNING",
    "availabilityPct": 92.5,
    "healthScore": 87.0,
    "lastSeen": "2024-01-01T12:00:00Z"
  }
]
```
