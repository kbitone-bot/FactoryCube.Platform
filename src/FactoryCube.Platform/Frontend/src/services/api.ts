import axios from 'axios'
import type { Project, Dataset, SyntheticJob, Experiment, QualityResult } from '@/types'

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

export default api

export const projectApi = {
  getAll: () => api.get('/projects').then(r => r.data as Project[]),
  getById: (id: string) => api.get(`/projects/${id}`).then(r => r.data as Project),
  create: (data: Partial<Project>) => api.post('/projects', data).then(r => r.data as Project),
}

export const datasetApi = {
  getByProject: (projectId: string) => api.get('/datasets?projectId=' + projectId).then(r => r.data as Dataset[]),
  getById: (id: string) => api.get(`/datasets/${id}`).then(r => r.data as Dataset),
  create: (data: Partial<Dataset>) => api.post('/datasets', data).then(r => r.data as Dataset),
  upload: (id: string, file: File) => {
    const form = new FormData()
    form.append('file', file)
    return api.post(`/datasets/${id}/upload`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then(r => r.data)
  },
  ingest: (id: string) => api.post(`/datasets/${id}/ingest`).then(r => r.data),
}

export const syntheticApi = {
  getJobs: (projectId: string) => api.get(`/synthetic/projects/${projectId}/jobs`).then(r => r.data as SyntheticJob[]),
  createJob: (data: Partial<SyntheticJob>) => api.post('/synthetic/jobs', data).then(r => r.data as SyntheticJob),
}

export const mlApi = {
  getExperiments: (projectId: string) => api.get(`/ml/projects/${projectId}/experiments`).then(r => r.data as Experiment[]),
  createExperiment: (data: Partial<Experiment>) => api.post('/ml/experiments', data).then(r => r.data as Experiment),
  startTrain: (id: string) => api.post(`/ml/experiments/${id}/train`).then(r => r.data),
}

export const qualityApi = {
  runCheck: (datasetId: string) => api.post(`/quality/datasets/${datasetId}/check`).then(r => r.data),
  getLatest: (datasetId: string) => api.get(`/quality/datasets/${datasetId}/latest`).then(r => r.data),
}

export const dashboardApi = {
  getSnapshot: (projectId: string) => api.get(`/dashboard/projects/${projectId}/snapshot`).then(r => r.data),
  getEquipmentStatus: (projectId: string) => api.get(`/dashboard/projects/${projectId}/equipment-status`).then(r => r.data),
}
