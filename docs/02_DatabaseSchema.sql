-- FactoryCube.Platform Database Schema
-- PostgreSQL 15+
-- Naming: fc_ prefix for all tables

-- 1. Project & Dataset Management
CREATE TABLE fc_project (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    equipment_type VARCHAR(50) NOT NULL CHECK (equipment_type IN ('PLC','TEST_EQUIPMENT','HYBRID')),
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE' CHECK (status IN ('ACTIVE','ARCHIVED','DELETED')),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100) NOT NULL
);
CREATE INDEX idx_fc_project_status ON fc_project(status);
CREATE INDEX idx_fc_project_equipment_type ON fc_project(equipment_type);

CREATE TABLE fc_dataset (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID NOT NULL REFERENCES fc_project(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    source_type VARCHAR(20) NOT NULL CHECK (source_type IN ('UPLOAD','WATCHER','API','SYNTHETIC')),
    record_count BIGINT DEFAULT 0,
    time_range_start TIMESTAMPTZ,
    time_range_end TIMESTAMPTZ,
    schema_detected JSONB,
    quality_score NUMERIC(5,2),
    status VARCHAR(20) NOT NULL DEFAULT 'DRAFT' CHECK (status IN ('DRAFT','PROCESSING','READY','FAILED','ARCHIVED')),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_dataset_project ON fc_dataset(project_id);
CREATE INDEX idx_fc_dataset_status ON fc_dataset(status);

CREATE TABLE fc_dataset_file (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    file_name VARCHAR(500) NOT NULL,
    file_path VARCHAR(1000) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    file_format VARCHAR(10) NOT NULL CHECK (file_format IN ('CSV','JSON','XLSX')),
    row_count BIGINT,
    checksum VARCHAR(64),
    upload_status VARCHAR(20) NOT NULL DEFAULT 'PENDING' CHECK (upload_status IN ('PENDING','PROCESSING','SUCCESS','FAILED')),
    error_message TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_dataset_file_dataset ON fc_dataset_file(dataset_id);

CREATE TABLE fc_schema_mapping (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID NOT NULL REFERENCES fc_project(id) ON DELETE CASCADE,
    mapping_name VARCHAR(200) NOT NULL,
    source_format VARCHAR(20) NOT NULL,
    mappings JSONB NOT NULL DEFAULT '{}',
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_schema_mapping_project ON fc_schema_mapping(project_id);

-- 2. Tag Masters
CREATE TABLE fc_plc_tag_master (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tag_code VARCHAR(100) NOT NULL UNIQUE,
    tag_name VARCHAR(200) NOT NULL,
    category VARCHAR(50) NOT NULL CHECK (category IN ('POSITION','SPINDLE','FEED','LOAD','TIME','COUNTER','ALARM','PMC','OTHER')),
    unit VARCHAR(20),
    data_type VARCHAR(20) NOT NULL CHECK (data_type IN ('NUMERIC','STRING','BOOLEAN','TIMESTAMP')),
    min_value NUMERIC,
    max_value NUMERIC,
    is_required BOOLEAN NOT NULL DEFAULT FALSE,
    standard_field VARCHAR(100),
    description TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_plc_tag_category ON fc_plc_tag_master(category);

CREATE TABLE fc_equipment_tag_master (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tag_code VARCHAR(100) NOT NULL UNIQUE,
    tag_name VARCHAR(200) NOT NULL,
    category VARCHAR(50) NOT NULL CHECK (category IN ('COMMON','UI_MEASURE','DERIVED','STATUS')),
    unit VARCHAR(20),
    data_type VARCHAR(20) NOT NULL CHECK (data_type IN ('NUMERIC','STRING','BOOLEAN','TIMESTAMP')),
    min_value NUMERIC,
    max_value NUMERIC,
    standard_field VARCHAR(100),
    description TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_equip_tag_category ON fc_equipment_tag_master(category);

-- 3. Data Records
CREATE TABLE fc_raw_record (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    source_file_id UUID REFERENCES fc_dataset_file(id),
    raw_data JSONB NOT NULL DEFAULT '{}',
    raw_hash VARCHAR(64),
    ingestion_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    seq_no BIGINT NOT NULL
);
CREATE INDEX idx_fc_raw_record_dataset ON fc_raw_record(dataset_id);
CREATE INDEX idx_fc_raw_record_seq ON fc_raw_record(dataset_id, seq_no);
CREATE INDEX idx_fc_raw_record_ingestion ON fc_raw_record USING BRIN(ingestion_time);

CREATE TABLE fc_normalized_record (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    record_time TIMESTAMPTZ NOT NULL,
    equipment_id VARCHAR(100) NOT NULL,
    source_type VARCHAR(20) NOT NULL,
    tag_values JSONB NOT NULL DEFAULT '{}',
    state VARCHAR(20),
    quality_flags JSONB NOT NULL DEFAULT '[]',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_norm_dataset_time ON fc_normalized_record(dataset_id, record_time);
CREATE INDEX idx_fc_norm_equipment ON fc_normalized_record(equipment_id, record_time);

CREATE TABLE fc_timeseries_1s (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    equipment_id VARCHAR(100) NOT NULL,
    ts TIMESTAMPTZ NOT NULL,
    abs_pos_1 NUMERIC, abs_pos_2 NUMERIC, abs_pos_3 NUMERIC, abs_pos_4 NUMERIC, abs_pos_5 NUMERIC,
    mach_pos_1 NUMERIC, mach_pos_2 NUMERIC, mach_pos_3 NUMERIC, mach_pos_4 NUMERIC, mach_pos_5 NUMERIC,
    rel_pos_1 NUMERIC, rel_pos_2 NUMERIC, rel_pos_3 NUMERIC, rel_pos_4 NUMERIC, rel_pos_5 NUMERIC,
    dist_togo_1 NUMERIC, dist_togo_2 NUMERIC, dist_togo_3 NUMERIC, dist_togo_4 NUMERIC, dist_togo_5 NUMERIC,
    spindle_speed NUMERIC,
    feed_rate NUMERIC,
    spindle_load NUMERIC,
    servo_load_1 NUMERIC, servo_load_2 NUMERIC, servo_load_3 NUMERIC, servo_load_4 NUMERIC, servo_load_5 NUMERIC,
    cycle_time_sec NUMERIC,
    part_counter INTEGER,
    power_on_time NUMERIC,
    run_time_sec NUMERIC,
    cut_time_sec NUMERIC,
    alarm_count INTEGER DEFAULT 0,
    proc_cpu_pct NUMERIC,
    proc_mem_ws_mb NUMERIC,
    proc_disk_read_bps NUMERIC,
    proc_disk_write_bps NUMERIC,
    host_cpu_pct NUMERIC,
    host_mem_pct NUMERIC,
    host_uptime_sec NUMERIC,
    pressure NUMERIC,
    temperature NUMERIC,
    air_pressure NUMERIC,
    custom_text_16 VARCHAR(100),
    custom_text_17 VARCHAR(100),
    state VARCHAR(20),
    anomaly_score NUMERIC,
    health_score NUMERIC,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_ts1s_dataset ON fc_timeseries_1s(dataset_id, ts);
CREATE INDEX idx_fc_ts1s_equip ON fc_timeseries_1s(equipment_id, ts);

CREATE TABLE fc_timeseries_1m (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    equipment_id VARCHAR(100) NOT NULL,
    ts TIMESTAMPTZ NOT NULL,
    spindle_speed_avg NUMERIC, spindle_speed_min NUMERIC, spindle_speed_max NUMERIC,
    spindle_load_avg NUMERIC, spindle_load_max NUMERIC,
    feed_rate_avg NUMERIC,
    cycle_time_avg NUMERIC, cycle_time_max NUMERIC,
    part_counter_delta INTEGER,
    proc_cpu_avg NUMERIC, proc_cpu_max NUMERIC,
    proc_mem_avg NUMERIC, proc_mem_max NUMERIC,
    host_cpu_avg NUMERIC, host_cpu_max NUMERIC,
    host_mem_avg NUMERIC, host_mem_max NUMERIC,
    pressure_avg NUMERIC, temperature_avg NUMERIC, air_pressure_avg NUMERIC,
    alarm_count INTEGER DEFAULT 0,
    restart_count INTEGER DEFAULT 0,
    state_mode VARCHAR(20),
    anomaly_score_avg NUMERIC,
    health_score_avg NUMERIC,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_ts1m_dataset ON fc_timeseries_1m(dataset_id, ts);
CREATE INDEX idx_fc_ts1m_equip ON fc_timeseries_1m(equipment_id, ts);

-- 4. Event & State History
CREATE TABLE fc_event_hist (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    equipment_id VARCHAR(100) NOT NULL,
    event_time TIMESTAMPTZ NOT NULL,
    event_type VARCHAR(50) NOT NULL,
    event_subtype VARCHAR(50),
    severity VARCHAR(20) NOT NULL CHECK (severity IN ('INFO','WARNING','CRITICAL')),
    details JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_event_dataset ON fc_event_hist(dataset_id, event_time);
CREATE INDEX idx_fc_event_equip ON fc_event_hist(equipment_id, event_time);

CREATE TABLE fc_state_hist (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    equipment_id VARCHAR(100) NOT NULL,
    state_start TIMESTAMPTZ NOT NULL,
    state_end TIMESTAMPTZ,
    state VARCHAR(20) NOT NULL CHECK (state IN ('OFF','READY','RUNNING','IDLE','HANG','ALARM','ERROR','MAINTENANCE')),
    duration_sec NUMERIC,
    transition_reason VARCHAR(200),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_state_dataset ON fc_state_hist(dataset_id, state_start);
CREATE INDEX idx_fc_state_equip ON fc_state_hist(equipment_id, state_start);

-- 5. Quality Engine
CREATE TABLE fc_quality_rule (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID REFERENCES fc_project(id) ON DELETE SET NULL,
    rule_name VARCHAR(200) NOT NULL,
    rule_type VARCHAR(20) NOT NULL CHECK (rule_type IN ('SCHEMA','VALUE','TIMESERIES','DOMAIN')),
    target_fields TEXT[],
    condition_expr TEXT NOT NULL,
    severity VARCHAR(20) NOT NULL DEFAULT 'WARNING' CHECK (severity IN ('INFO','WARNING','CRITICAL')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_quality_rule_project ON fc_quality_rule(project_id);
CREATE INDEX idx_fc_quality_rule_type ON fc_quality_rule(rule_type);

CREATE TABLE fc_quality_result (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    rule_id UUID REFERENCES fc_quality_rule(id),
    check_batch_id UUID NOT NULL,
    check_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    issue_level VARCHAR(20) NOT NULL CHECK (issue_level IN ('ROW','BATCH','DATASET')),
    affected_rows BIGINT,
    issue_count BIGINT,
    sample_issues JSONB,
    score NUMERIC(5,2),
    verdict VARCHAR(20) NOT NULL CHECK (verdict IN ('PASS','WARNING','REJECT')),
    details JSONB
);
CREATE INDEX idx_fc_quality_dataset ON fc_quality_result(dataset_id, check_time);
CREATE INDEX idx_fc_quality_batch ON fc_quality_result(check_batch_id);

-- 6. Synthetic Data
CREATE TABLE fc_synthetic_job (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID NOT NULL REFERENCES fc_project(id) ON DELETE CASCADE,
    job_name VARCHAR(200) NOT NULL,
    scenario_config JSONB NOT NULL DEFAULT '{}',
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ NOT NULL,
    equipment_count INTEGER NOT NULL DEFAULT 1,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING' CHECK (status IN ('PENDING','RUNNING','COMPLETED','FAILED','CANCELLED')),
    progress_pct INTEGER DEFAULT 0,
    output_dataset_id UUID REFERENCES fc_dataset(id),
    error_message TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMPTZ
);
CREATE INDEX idx_fc_synjob_project ON fc_synthetic_job(project_id);
CREATE INDEX idx_fc_synjob_status ON fc_synthetic_job(status);

CREATE TABLE fc_synthetic_dataset (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    job_id UUID NOT NULL REFERENCES fc_synthetic_job(id) ON DELETE CASCADE,
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id) ON DELETE CASCADE,
    generation_method VARCHAR(50) NOT NULL,
    scenario_mix JSONB,
    seed INTEGER,
    noise_level NUMERIC(5,4),
    drift_level NUMERIC(5,4),
    label_fields TEXT[],
    validation_score NUMERIC(5,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_syndataset_job ON fc_synthetic_dataset(job_id);

CREATE TABLE fc_synthetic_validation (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    synthetic_dataset_id UUID NOT NULL REFERENCES fc_synthetic_dataset(id) ON DELETE CASCADE,
    original_dataset_id UUID REFERENCES fc_dataset(id),
    metric_name VARCHAR(100) NOT NULL,
    original_value NUMERIC,
    synthetic_value NUMERIC,
    difference_pct NUMERIC,
    pass_threshold NUMERIC,
    is_passed BOOLEAN NOT NULL,
    details JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_synval_syn ON fc_synthetic_validation(synthetic_dataset_id);

-- 7. ML Pipeline
CREATE TABLE fc_ml_experiment (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID NOT NULL REFERENCES fc_project(id) ON DELETE CASCADE,
    experiment_name VARCHAR(200) NOT NULL,
    task_type VARCHAR(20) NOT NULL CHECK (task_type IN ('CLASSIFICATION','REGRESSION','ANOMALY_DETECTION','TIME_SERIES')),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id),
    model_type VARCHAR(50) NOT NULL,
    feature_config JSONB,
    hyperparameters JSONB,
    train_config JSONB,
    status VARCHAR(20) NOT NULL DEFAULT 'DRAFT' CHECK (status IN ('DRAFT','RUNNING','COMPLETED','FAILED')),
    best_model_path VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_ml_exp_project ON fc_ml_experiment(project_id);
CREATE INDEX idx_fc_ml_exp_status ON fc_ml_experiment(status);

CREATE TABLE fc_ml_model_registry (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    experiment_id UUID NOT NULL REFERENCES fc_ml_experiment(id) ON DELETE CASCADE,
    model_version INTEGER NOT NULL,
    model_path VARCHAR(500) NOT NULL,
    metrics JSONB,
    is_deployed BOOLEAN NOT NULL DEFAULT FALSE,
    deployed_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_ml_reg_exp ON fc_ml_model_registry(experiment_id);

CREATE TABLE fc_ml_run_metric (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    experiment_id UUID NOT NULL REFERENCES fc_ml_experiment(id) ON DELETE CASCADE,
    run_id VARCHAR(100) NOT NULL,
    epoch INTEGER,
    metric_name VARCHAR(100) NOT NULL,
    metric_value NUMERIC NOT NULL,
    step INTEGER,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_ml_metric_exp ON fc_ml_run_metric(experiment_id, metric_name);

CREATE TABLE fc_prediction_result (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    experiment_id UUID NOT NULL REFERENCES fc_ml_experiment(id) ON DELETE CASCADE,
    model_registry_id UUID REFERENCES fc_ml_model_registry(id),
    dataset_id UUID NOT NULL REFERENCES fc_dataset(id),
    equipment_id VARCHAR(100) NOT NULL,
    prediction_time TIMESTAMPTZ NOT NULL,
    target_field VARCHAR(100),
    predicted_value NUMERIC,
    predicted_class VARCHAR(100),
    probability NUMERIC,
    anomaly_score NUMERIC,
    feature_snapshot JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_pred_exp ON fc_prediction_result(experiment_id, prediction_time);
CREATE INDEX idx_fc_pred_equip ON fc_prediction_result(equipment_id, prediction_time);

-- 8. Dashboard
CREATE TABLE fc_dashboard_snapshot (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID NOT NULL REFERENCES fc_project(id) ON DELETE CASCADE,
    snapshot_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    snapshot_type VARCHAR(50) NOT NULL,
    kpis JSONB NOT NULL DEFAULT '{}',
    chart_data JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_fc_dash_project ON fc_dashboard_snapshot(project_id, snapshot_time);

-- Update trigger for updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_fc_project_updated BEFORE UPDATE ON fc_project FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_fc_dataset_updated BEFORE UPDATE ON fc_dataset FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_fc_schema_mapping_updated BEFORE UPDATE ON fc_schema_mapping FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_fc_ml_experiment_updated BEFORE UPDATE ON fc_ml_experiment FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
