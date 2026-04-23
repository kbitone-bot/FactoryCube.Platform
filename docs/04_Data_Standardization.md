# 데이터 표준화 설계

## 1. 원천 공통데이터 → 표준 필드

| 원천 필드 | 표준 필드 | 단위 | 타입 |
|-----------|-----------|------|------|
| process_cpu_percent | proc_cpu_pct | % | NUMERIC |
| process_memory_working_set_mb | proc_mem_ws_mb | MB | NUMERIC |
| process_disk_read_bytes_sec | proc_disk_read_bps | bytes/s | NUMERIC |
| process_disk_write_bytes_sec | proc_disk_write_bps | bytes/s | NUMERIC |
| memory_percent | host_mem_pct | % | NUMERIC |
| uptime_sec | host_uptime_sec | sec | NUMERIC |
| cpu_percent | host_cpu_pct | % | NUMERIC |
| memory_used_mb | host_mem_used_mb | MB | NUMERIC |
| net_sent_bytes_sec | net_sent_bps | bytes/s | NUMERIC |
| net_recv_bytes_sec | net_recv_bps | bytes/s | NUMERIC |

## 2. 원천 UI/측정데이터 → 표준 필드

| 원천 필드 | 표준 필드 | 단위 | 타입 |
|-----------|-----------|------|------|
| 압력 | pressure | MPa | NUMERIC |
| 온도 | temperature | °C | NUMERIC |
| 기압 | air_pressure | MPa | NUMERIC |
| 전류 | current | A | NUMERIC |
| 전압 | voltage | V | NUMERIC |
| Text_16 | custom_text_16 | - | STRING |
| Text_17 | custom_text_17 | - | STRING |

## 3. PLC 데이터 → 표준 필드

| 원천 필드 | 표준 필드 | 단위 | 타입 |
|-----------|-----------|------|------|
| Absolute_Position1..5 | abs_pos_1..5 | mm | NUMERIC |
| Machine_Position1..5 | mach_pos_1..5 | mm | NUMERIC |
| Relative_Position1..5 | rel_pos_1..5 | mm | NUMERIC |
| Distance_Togo1..5 | dist_togo_1..5 | mm | NUMERIC |
| Spindle Speed(rpm) | spindle_speed | rpm | NUMERIC |
| Current Fead Rate | feed_rate | mm/min | NUMERIC |
| Current Spindle Load[0] | spindle_load | % | NUMERIC |
| Server Load1..5 | servo_load_1..5 | % | NUMERIC |
| CycleTime(Sec 초단위) | cycle_time_sec | sec | NUMERIC |
| PartCounter1 | part_counter | count | INTEGER |
| PowerOnTime 통전적산치 | power_on_time | sec | NUMERIC |

## 4. 파생 필드 생성 규칙

```csharp
// 효율적 가동 여부
is_running_effective = state == "RUNNING" && spindle_load > 10;

// 유휴 후보
is_idle_candidate = state == "RUNNING" && proc_cpu_pct <= 3 && proc_disk_read_bps < 1000;

// 공회전 후보
is_air_cut_candidate = spindle_speed > 0 && spindle_load < 5;

// 프로세스 건강도 (0-100)
process_health_score = 100 
    - proc_cpu_pct * 0.3 
    - (proc_mem_ws_mb / 1024) * 5 
    - (restart_count * 10);

// 장비 건강도
equipment_health_score = (process_health_score + (100 - spindle_load)) / 2;

// 사이클타임 변화량
cycle_time_delta = cycle_time_sec(t) - cycle_time_sec(t-1);

// 메모리 증가율
memory_growth_rate = proc_mem_ws_mb(t) - proc_mem_ws_mb(t-1);

// CPU 스파이크
cpu_spike_flag = proc_cpu_pct > percentile(proc_cpu_pct, 95);

// 재시작 급증
restart_spike_flag = restart_count(t) - restart_count(t-1) > 2;

// 수집 누락
missing_interval_flag = ts(t) - ts(t-1) > 2 seconds;

// 값 고정
flatline_flag = max(spindle_speed(t-9..t)) - min(spindle_speed(t-9..t)) < 0.01;

// 드리프트 점수
drift_score = linear_regression_slope(pressure(t-59..t));

// 룰 기반 이상 점수
anomaly_score_rule_based = 
    is_air_cut_candidate * 0.3 +
    cpu_spike_flag * 0.3 +
    flatline_flag * 0.2 +
    (temperature > 45) * 0.2;
```

## 5. 표준 상태 판정 로직

```csharp
if (process == null || !running) return "OFF";
if (is_hung == true) return "HANG";
if (alarm_count > 0) return "ALARM";
if (restart_spike_flag) return "ERROR";
if (running && (proc_cpu_pct > 3 || spindle_speed > 0 || feed_rate > 0)) return "RUNNING";
if (running && proc_cpu_pct <= 3 && spindle_speed == 0) return "IDLE";
if (!running && host_uptime_sec > 0) return "READY";
return "MAINTENANCE"; // 수동 입력 시
```
