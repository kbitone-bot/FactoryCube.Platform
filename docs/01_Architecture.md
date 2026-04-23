# FactoryCube.Platform - 시스템 아키텍처

## 1. 전체 목표 요약

FactoryCube.Platform은 **PLC 장비 데이터**와 **시험장비 공통/가공 데이터**를 통합 수집·가공·분석하는 산업용 데이터 플랫폼이다.

핵심 가치:
- 원천 데이터 적재 → 표준화 → 품질검증
- 합성데이터 대량 생성 및 검증
- AI/ML 기반 예측·이상탐지·분류
- 웹 기반 운영 대시보드
- 프로젝트 단위 재사용 가능한 제품 구조

## 2. 시스템 아키텍처

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Client Layer                                    │
│  React + TypeScript (Dashboard, Data Explorer, AI Experiment, Synthetic UI)  │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │ REST API
┌─────────────────────────────────────────────────────────────────────────────┐
│                           WebApi (.NET 8)                                    │
│  Controllers → Services → Repositories → EF Core → PostgreSQL               │
│  BackgroundService (File Watcher, Job Queue, Python Runner)                 │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Application Layer                                    │
│  DTOs, Validators, Business Rules, Feature Mapping, Quality Engine          │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Domain Layer                                       │
│  Entities, Enums, Value Objects, Repository Interfaces                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Infrastructure Layer                                  │
│  EF Core DbContext, Repositories, File Upload, Python Process Runner        │
│  Background Jobs (Synthetic Generation, Training, Inference)                │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Python Pipeline Layer                                 │
│  synthetic/  validation/  features/  train/  infer/  report/                 │
│  CLI 기반 Job 실행, config.json + input.csv → output.csv/artifacts          │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Data Layer                                         │
│  PostgreSQL (Primary), File Storage (CSV/JSON/XLSX artifacts)               │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 3. 배치/Job 아키텍처

```
[FileWatcher] ──→ [IngestJob] ──→ [NormalizeJob] ──→ [QualityJob]
                                            │
                                            ↓
[SyntheticJob] ←── [ScenarioConfig] ←── [FeatureJob]
      │
      ↓
[TrainJob] ←── [ExperimentConfig]
      │
      ↓
[InferenceJob] ──→ [DashboardSnapshot]
```

## 4. C# ↔ Python 연동

- 방식: Process 호출 (CLI) + 파일 기반 데이터 교환
- C#: `PythonRunnerService`가 `python main.py --job-type synthetic --config config.json` 실행
- Python: JSON config 읽고, CSV input 처리 후 결과 CSV + metric JSON 생성
- 확장: 향후 gRPC 또는 메시지 큐(RabbitMQ)로 마이그레이션 가능한 인터페이스 설계
