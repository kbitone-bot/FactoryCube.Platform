export interface Project {
  id: string
  name: string
  description?: string
  equipmentType: string
  status: string
  createdAt: string
  updatedAt: string
  createdBy: string
}

export interface Dataset {
  id: string
  projectId: string
  name: string
  description?: string
  sourceType: string
  recordCount: number
  timeRangeStart?: string
  timeRangeEnd?: string
  qualityScore?: number
  status: string
  createdAt: string
}

export interface SyntheticJob {
  id: string
  projectId: string
  jobName: string
  startTime: string
  endTime: string
  equipmentCount: number
  status: string
  progressPct: number
  outputDatasetId?: string
  createdAt: string
  completedAt?: string
}

export interface Experiment {
  id: string
  projectId: string
  experimentName: string
  taskType: string
  datasetId: string
  modelType: string
  status: string
  bestModelPath?: string
  createdAt: string
}

export interface QualityResult {
  id: string
  datasetId: string
  checkTime: string
  issueLevel: string
  affectedRows?: number
  issueCount: number
  score?: number
  verdict: string
}

export interface DashboardKpi {
  label: string
  value: number
  unit?: string
  changePct?: number
  trend?: string
}
