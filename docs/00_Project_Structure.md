# 프로젝트 폴더 구조

```
FactoryCube.Platform/
├── docs/
│   ├── 00_Project_Structure.md
│   ├── 01_Architecture.md
│   ├── 02_DatabaseSchema.sql
│   ├── 03_API_Specification.md
│   ├── 04_Data_Standardization.md
│   ├── 05_Quality_Validation.md
│   ├── 06_Synthetic_Data_Design.md
│   ├── 07_AI_Pipeline.md
│   ├── 08_Execution_Guide.md
│   ├── 09_Test_Strategy.md
│   └── 10_Future_Roadmap.md
│
├── src/
│   └── FactoryCube.Platform/
│       ├── Backend/
│       │   ├── FactoryCube.Platform.sln
│       │   ├── FactoryCube.Core/
│       │   │   ├── FactoryCube.Core.csproj
│       │   │   ├── appsettings.json
│       │   │   ├── Program.cs
│       │   │   ├── Domain/
│       │   │   │   ├── Entities/
│       │   │   │   │   ├── Project.cs
│       │   │   │   │   ├── Dataset.cs
│       │   │   │   │   ├── DatasetFile.cs
│       │   │   │   │   ├── SchemaMapping.cs
│       │   │   │   │   ├── RawRecord.cs
│       │   │   │   │   ├── NormalizedRecord.cs
│       │   │   │   │   ├── QualityRule.cs
│       │   │   │   │   ├── QualityResult.cs
│       │   │   │   │   ├── SyntheticJob.cs
│       │   │   │   │   ├── SyntheticDataset.cs
│       │   │   │   │   ├── SyntheticValidation.cs
│       │   │   │   │   ├── MlExperiment.cs
│       │   │   │   │   ├── MlModelRegistry.cs
│       │   │   │   │   ├── MlRunMetric.cs
│       │   │   │   │   ├── PredictionResult.cs
│       │   │   │   │   └── DashboardSnapshot.cs
│       │   │   │   ├── Enums/
│       │   │   │   ├── Interfaces/
│       │   │   │   │   └── IRepository.cs
│       │   │   │   ├── ValueObjects/
│       │   │   │   └── (도메인 값 객체)
│       │   │   ├── Application/
│       │   │   │   ├── DTOs/
│       │   │   │   │   ├── ProjectDtos.cs
│       │   │   │   │   ├── DatasetDtos.cs
│       │   │   │   │   ├── SyntheticDtos.cs
│       │   │   │   │   ├── MlDtos.cs
│       │   │   │   │   ├── QualityDtos.cs
│       │   │   │   │   └── DashboardDtos.cs
│       │   │   │   ├── Interfaces/
│       │   │   │   │   ├── IProjectService.cs
│       │   │   │   │   ├── IDatasetService.cs
│       │   │   │   │   ├── ISyntheticService.cs
│       │   │   │   │   ├── IMlService.cs
│       │   │   │   │   ├── IQualityService.cs
│       │   │   │   │   └── IDashboardService.cs
│       │   │   │   ├── Services/
│       │   │   │   │   ├── ProjectService.cs
│       │   │   │   │   ├── DatasetService.cs
│       │   │   │   │   ├── SyntheticService.cs
│       │   │   │   │   ├── MlService.cs
│       │   │   │   │   ├── QualityService.cs
│       │   │   │   │   └── DashboardService.cs
│       │   │   │   └── Validators/
│       │   │   ├── Infrastructure/
│       │   │   │   ├── Data/
│       │   │   │   │   ├── FactoryCubeDbContext.cs
│       │   │   │   │   └── Repositories/
│       │   │   │   │       ├── Repository.cs
│       │   │   │   │       ├── ProjectRepository.cs
│       │   │   │   │       ├── DatasetRepository.cs
│       │   │   │   │       ├── SyntheticJobRepository.cs
│       │   │   │   │       └── MlExperimentRepository.cs
│       │   │   │   ├── Jobs/
│       │   │   │   │   └── SyntheticJobBackgroundService.cs
│       │   │   │   ├── PythonRunner/
│       │   │   │   │   └── PythonRunnerService.cs
│       │   │   │   ├── FileUpload/
│       │   │   │   └── (파일 업로드 헬퍼)
│       │   │   │   └── Mapping/
│       │   │   └── WebApi/
│       │   │       ├── Controllers/
│       │   │       │   ├── ProjectsController.cs
│       │   │       │   ├── DatasetsController.cs
│       │   │       │   ├── SyntheticController.cs
│       │   │       │   ├── MlController.cs
│       │   │       │   ├── QualityController.cs
│       │   │       │   └── DashboardController.cs
│       │   │       ├── Models/
│       │   │       ├── Middleware/
│       │   │       └── Extensions/
│       │   │
│       │   └── FactoryCube.Core.Tests/
│       │       ├── FactoryCube.Core.Tests.csproj
│       │       └── (단위/통합 테스트)
│       │
│       └── Frontend/
│           ├── package.json
│           ├── tsconfig.json
│           ├── vite.config.ts
│           ├── index.html
│           └── src/
│               ├── main.tsx
│               ├── App.tsx
│               ├── types/
│               │   └── index.ts
│               ├── services/
│               │   └── api.ts
│               ├── components/
│               │   └── Layout.tsx
│               ├── pages/
│               │   ├── ProjectListPage.tsx
│               │   ├── ProjectDetailPage.tsx
│               │   ├── DatasetPage.tsx
│               │   ├── SyntheticPage.tsx
│               │   ├── QualityPage.tsx
│               │   ├── MlPage.tsx
│               │   └── DashboardPage.tsx
│               ├── hooks/
│               ├── utils/
│               └── styles/
│
└── data/                    # 로컬 파일 저장소 (gitignore 권장)
    ├── uploads/
    ├── synthetic/
    └── artifacts/
```

## Python Pipeline (Backend 난독)
```
Backend/FactoryCube.Python/
├── main.py
├── requirements.txt
├── synthetic/
│   ├── __init__.py
│   ├── generator.py
│   └── scenario.py
├── validation/
│   ├── __init__.py
│   └── validator.py
├── features/
│   ├── __init__.py
│   └── engineering.py
├── train/
│   ├── __init__.py
│   └── trainer.py
├── infer/
│   ├── __init__.py
│   └── inference.py
└── report/
    ├── __init__.py
    └── (리포트 생성 모듈)
```
