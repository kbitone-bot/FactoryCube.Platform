# 향후 확장 방향

## MVP (현재 단계)
- [x] 데이터 업로드/적재 (CSV/JSON/XLSX)
- [x] 표준화/가공 (PLC + Test Equipment)
- [x] 품질 검증 (스키마/값/시계열/도메인)
- [x] 룰 기반 합성데이터 생성
- [x] 합성데이터 검증 (분포/상태/도메인)
- [x] 기본 ML 학습 (XGBoost/RandomForest/IsolationForest)
- [x] 대시보드 가시화

## 2차 고도화
- [ ] 상태 전이 고도화 (HMM, Markov Chain 기반 확률 모델)
- [ ] 고급 이상탐지 (LSTM Autoencoder, Transformer)
- [ ] AutoML 보조 (Optuna 기반 하이퍼파라미터 탐색)
- [ ] SHAP Explainability 통합
- [ ] 모델 레지스트리 고도화 (A/B 테스트, 롤백)
- [ ] 스케줄링 및 운영 자동화 (Quartz.NET + Hangfire)

## 3차 아키텍처 확장
- [ ] 마이크로서비스 분리 (Data Ingestion, Synthetic, ML, Dashboard)
- [ ] 메시지 큐 도입 (RabbitMQ / Kafka)
- [ ] 시계열 DB 확장 (InfluxDB / TimescaleDB)
- [ ] 실시간 스트리밍 (SignalR + WebSocket)
- [ ] 컨테이너화 (Docker + Kubernetes)
- [ ] gRPC 기반 C# ↔ Python 연동
- [ ] 멀티테넌시 지원

## 산업 확장
- [ ] PLC 종류 추가 (Siemens, Mitsubishi, Beckhoff)
- [ ] 시험장비 종류 추가 (H/W 벤더별 어댑터)
- [ ] OPC-UA 수집 연동
- [ ] Edge 연동 (공장 현장 게이트웨이)
