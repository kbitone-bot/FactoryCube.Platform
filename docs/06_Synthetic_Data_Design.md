# 합성데이터 생성기 설계

## 1. 생성 아키텍처

```
Scenario Library
    ↓
State Transition Engine  →  상태 시퀀스 생성
    ↓
Signal Generator         →  기본 신호 + 주기성 + 노이즈
    ↓
Noise Injector           →  가우시안/아비트러리 노이즈
    ↓
Anomaly Injector         →  spike, drift, stuck, dropout 등
    ↓
Label Generator          →  상태/이상/시나리오/위험도 라벨
    ↓
Validation Engine        →  원본 대비 분포/상관관계/도메인 검증
    ↓
Exporter                 →  CSV/JSON/Parquet
```

## 2. 상태 전이 엔진

```
OFF → READY → RUNNING → IDLE → RUNNING → OFF
RUNNING → ALARM → STOP
RUNNING → HANG → RECOVERED (READY/RUNNING)
RUNNING → ERROR → RESTART → READY
READY → IDLE → RUNNING
```

### 상태별 지속 시간 분포
| 상태 | 최소(초) | 최대(초) | 분포 |
|------|----------|----------|------|
| OFF | 300 | 1800 | uniform |
| READY | 60 | 300 | uniform |
| RUNNING | 600 | 7200 | normal |
| IDLE | 120 | 1800 | uniform |
| HANG | 60 | 600 | exponential |
| ALARM | 30 | 300 | uniform |
| ERROR | 60 | 600 | uniform |
| MAINTENANCE | 300 | 3600 | uniform |

## 3. 신호 생성 규칙

### Spindle Speed
```
spindle_speed(t) = baseline_speed * state_factor + periodic(t) + noise(t)
- baseline_speed: 3000 rpm (정상)
- state_factor: RUNNING=1.0, IDLE=0.0, HANG=0.0, AIR_CUT=0.8
- periodic(t): 50 * sin(2π * t / 60)
- noise(t): N(0, noise_level * baseline_speed)
```

### Spindle Load
```
spindle_load(t) = f(spindle_speed, feed_rate, cutting_phase, anomaly_factor)
- 기본: 25 + (speed / 3000) * 30
- 공회전: 5 + noise
- 고부하: 기본 * 1.5
- anomaly_factor: spike 시 2~4배
```

### Cycle Time
```
cycle_time_accum(t) = cycle_time_accum(t-1) + dt  (when RUNNING)
part_counter(t) += 1  (on cycle completion event)
cycle_completion_event: random() < 1.0 / baseline_cycle
```

### Process Memory (Memory Leak 시나리오)
```
proc_mem_ws_mb(t) = base_mem + leak_rate * elapsed_sec + random_noise
- base_mem: 120 MB
- leak_rate: 0.05 MB/sec (leak 시나리오), 0.0 (정상)
```

### Pressure/Temperature (Test Equipment)
```
pressure(t) = target_pressure ± noise ± drift
temperature(t) = 25 + (spindle_load / 100) * 15 + noise
drift(t) = drift_level * elapsed_sec
```

## 4. 이상 시나리오 주입

| 이상 유형 | 설명 | 파라미터 |
|-----------|------|----------|
| spike | 순간 값 급증 | multiplier: 2~4, duration: 1~3초 |
| drift | 서서히 값 이동 | drift_rate: 0.001~0.01 / sec |
| stuck-at | 값 고정 | duration: 10~300초 |
| dropout | 값 누락 (0 또는 null) | duration: 1~10초 |
| delayed transition | 상태 전이 지연 | delay: 30~300초 |
| restart burst | 재시작 반복 | count: 3~10회, interval: 5~30초 |
| hung interval | 응답 없음 지속 | duration: 60~600초 |
| alarm burst | 알람 연발 | count: 5~20회 |
| counter rollback | 카운터 감소 | drop: 1~100 |
| impossible combination | speed>0 & load=0 & state=RUNNING | - |

## 5. 라벨 생성

```
state_label: OFF | READY | RUNNING | IDLE | HANG | ALARM | ERROR | MAINTENANCE
anomaly_label: 0 | 1
scenario_label: normal | idle | air_cut | heavy_load | memory_leak | drift | spike | ...
failure_risk_level: LOW | MEDIUM | HIGH | CRITICAL
health_grade: A | B | C | D | F
```

## 6. 생성 옵션

```json
{
  "start_time": "2024-01-01T00:00:00Z",
  "end_time": "2024-01-08T00:00:00Z",
  "equipment_count": 10,
  "equipment_type": "PLC",
  "state_ratio": { "RUNNING": 0.6, "IDLE": 0.2, "OFF": 0.1, "ALARM": 0.05, "HANG": 0.05 },
  "anomaly_ratio": 0.05,
  "seed": 42,
  "noise_level": 0.02,
  "drift_level": 0.001,
  "scenario_mix": ["normal", "heavy_load", "memory_leak"],
  "label_include": true
}
```
