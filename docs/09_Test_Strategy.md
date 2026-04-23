# 테스트 전략

## 1. 단위 테스트 (C#)

- **Domain**: Entity 생성, 상태 전이, 유효성 검사
- **Application**: Service 로직, DTO 매핑, 비즈니스 규칙
- **Infrastructure**: Repository CRUD, DbContext 설정

프레임워크: xUnit + Moq + FluentAssertions

```bash
cd src/FactoryCube.Platform/Backend/FactoryCube.Core.Tests
dotnet test
```

## 2. 통합 테스트

- **API 통합**: Controller → Service → Repository → DB
- **파일 업로드/적재**: CSV/JSON/XLSX 업로드 후 RawRecord 적재 확인
- **Python 연동**: Process 호출 성공/실패, 결과 파일 존재 여부

## 3. Python 모듈 테스트

- **synthetic**: 생성된 데이터 row 수, 컬럼 존재, 상태 비율, 이상 비율
- **validation**: 원본-합성 분포 차이, 도메인 룰 위반 카운트
- **features**: lag/rolling 컬럼 생성 여부, 결측치 처리
- **train**: 모델 저장, metric 범위 (f1 > 0.5 등)

```bash
cd src/FactoryCube.Platform/Backend/FactoryCube.Python
pytest tests/ -v
```

## 4. E2E 테스트

- **시나리오**: 프로젝트 생성 → 데이터 업로드 → 적재 → 품질검증 → 합성데이터 생성 → 학습 → 대시보드 확인
- **도구**: Playwright 또는 Cypress (프론트엔드)

## 5. 성능 테스트

- **대용량 시계열**: 1대 장비, 1일분 (86,400 row) INSERT 속도
- **배치 처리**: 10만 row CSV 적재 시간
- **Python Job**: 1일분 합성데이터 생성 시간 < 30초

## 6. 품질 검증 테스트

- **스키마**: 필수 컬럼 누락 시 REJECT
- **값**: 음수 spindle_load 발생 시 WARNING
- **시계열**: timestamp 역전 발생 시 CRITICAL
- **도메인**: part_counter 감소 발생 시 CRITICAL
