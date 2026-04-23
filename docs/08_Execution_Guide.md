# 실행 방법

## 1. 사전 요구사항

### 백엔드
- .NET 8 SDK
- PostgreSQL 15+
- Windows Server 또는 Windows 10/11

### 프론트엔드
- Node.js 18+
- npm 또는 yarn

### Python
- Python 3.11
- pip

## 2. DB 설정

```bash
# PostgreSQL 설치 후
psql -U postgres -c "CREATE DATABASE factorycube;"
# 또는 SQL 파일 직접 실행
psql -U postgres -d factorycube -f docs/02_DatabaseSchema.sql
```

## 3. 백엔드 실행

```bash
cd src/FactoryCube.Platform/Backend/FactoryCube.Core
dotnet restore
dotnet ef migrations add InitialCreate  # (선택) EF Migrations 사용 시
dotnet run
# 기본 포트: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## 4. 프론트엔드 실행

```bash
cd src/FactoryCube.Platform/Frontend
npm install
npm run dev
# 기본 포트: http://localhost:3000
```

## 5. Python 환경 설정

```bash
cd src/FactoryCube.Platform/Backend/FactoryCube.Python
python -m venv venv
venv\Scripts\activate  # Windows
pip install -r requirements.txt
```

## 6. 합성데이터 생성 테스트 (CLI)

```bash
python main.py \
  --job-type synthetic \
  --config config_sample.json \
  --output ./output
```

config_sample.json 예시:
```json
{
  "start_time": "2024-01-01T00:00:00Z",
  "end_time": "2024-01-02T00:00:00Z",
  "equipment_count": 3,
  "scenario_config": {
    "scenarios": ["normal", "heavy_load"],
    "noise_level": 0.02,
    "drift_level": 0.001
  },
  "seed": 42,
  "noise_level": 0.02,
  "anomaly_ratio": 0.05
}
```

## 7. 전체 파이프라인 테스트

1. 프로젝트 생성 (Web UI 또는 POST /api/projects)
2. 데이터셋 등록 (POST /api/datasets)
3. 파일 업로드 (POST /api/datasets/{id}/upload)
4. 적재 실행 (POST /api/datasets/{id}/ingest)
5. 품질 검증 (POST /api/quality/datasets/{id}/check)
6. 합성데이터 Job 생성 (POST /api/synthetic/jobs)
7. 합성데이터 검증 결과 확인 (GET /api/synthetic/validations/{id})
8. ML 실험 생성 (POST /api/ml/experiments)
9. 학습 시작 (POST /api/ml/experiments/{id}/train)
10. 대시보드 확인 (GET /api/dashboard/projects/{id}/snapshot)
