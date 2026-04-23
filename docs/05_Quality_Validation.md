# 데이터 품질 검증 설계

## 1. 검증 유형

### 1.1 스키마 검증
- 필수 컬럼 존재 여부 (spindle_speed, ts 등)
- 타입 불일치 (문자열로 들어온 숫자)
- 컬럼 누락
- 중복 컬럼

### 1.2 값 검증
- null 비율 임계값 초과 (단일 컬럼 > 10%, 전체 > 5%)
- 음수 불가 필드의 음수값 (spindle_load, cycle_time_sec 등)
- 범위 초과 (spindle_speed > 20000 rpm)
- timestamp 역전 (ts(t) < ts(t-1))
- 1초 간격 이탈 (diff > 2초)

### 1.3 시계열 검증
- 간격 누락 (expected - actual > 0)
- 중복 시각 (count(ts) > 1)
- 평평한 값 지속 (10초 이상 동일값)
- 급격한 spike (|z-score| > 4)
- 장시간 0 고정 (60초 이상)
- 카운터 감소 (part_counter(t) < part_counter(t-1))
- 누적값 역행 (power_on_time(t) < power_on_time(t-1))

### 1.4 도메인 검증
```
spindle_speed > 0 AND spindle_load == 0  → 공회전 후보
spindle_speed == 0 AND cycle_time 증가   → 로직 이상 후보
part_counter 감소                        → 이상
PowerOnTime 감소                         → 이상
process_restart_count 급증               → 비정상
is_hung == true 지속                     → 장애
pressure / temperature 범위 초과         → 센서/데이터 오류
```

## 2. 품질 점수 산출

```
score = 100
- schema_penalty * 10
- null_penalty * 5
- range_penalty * 3
- timeseries_penalty * 2
- domain_penalty * 4

verdict:
  score >= 90 → PASS
  score >= 70 → WARNING
  score < 70  → REJECT
```

## 3. 검증 결과 구조

```json
{
  "checkBatchId": "uuid",
  "datasetId": "uuid",
  "checkTime": "2024-01-01T00:00:00Z",
  "overallScore": 87.5,
  "verdict": "PASS",
  "issues": [
    {
      "level": "ROW|BATCH|DATASET",
      "ruleId": "uuid",
      "ruleName": "spindle_load 음수 검사",
      "affectedRows": 3,
      "sample": [ { "row": 1024, "value": -5.2 } ]
    }
  ]
}
```
