import { useState } from 'react'
import { useQuery, useMutation } from 'react-query'
import { Typography, Button, TextField, Box, Paper, Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material'
import { mlApi } from '@/services/api'
import type { Experiment } from '@/types'

export default function MlPage() {
  const [projectId, setProjectId] = useState('')
  const [experimentName, setExperimentName] = useState('')
  const [datasetId, setDatasetId] = useState('')
  const { data: experiments, refetch } = useQuery(['experiments', projectId], () => mlApi.getExperiments(projectId), { enabled: !!projectId })
  const createMut = useMutation(mlApi.createExperiment, { onSuccess: () => refetch() })
  const trainMut = useMutation(mlApi.startTrain)

  const handleCreate = () => {
    createMut.mutate({
      projectId,
      experimentName,
      taskType: 'CLASSIFICATION',
      datasetId,
      modelType: 'XGBoost',
    })
  }

  return (
    <div>
      <Typography variant="h4" gutterBottom>AI 실험 관리</Typography>
      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
          <TextField label="Project ID" value={projectId} onChange={e => setProjectId(e.target.value)} />
          <TextField label="Experiment Name" value={experimentName} onChange={e => setExperimentName(e.target.value)} />
          <TextField label="Dataset ID" value={datasetId} onChange={e => setDatasetId(e.target.value)} />
          <Button variant="contained" onClick={handleCreate}>생성</Button>
        </Box>
      </Paper>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Task</TableCell>
              <TableCell>Model</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Action</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {experiments?.map((e) => (
              <TableRow key={e.id}>
                <TableCell>{e.experimentName}</TableCell>
                <TableCell>{e.taskType}</TableCell>
                <TableCell>{e.modelType}</TableCell>
                <TableCell>{e.status}</TableCell>
                <TableCell>
                  <Button size="small" onClick={() => trainMut.mutate(e.id)}>학습</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}
