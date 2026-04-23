import { useState } from 'react'
import { useQuery, useMutation } from 'react-query'
import { Typography, Button, TextField, Box, Paper, Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material'
import { syntheticApi } from '@/services/api'
import type { SyntheticJob } from '@/types'

export default function SyntheticPage() {
  const [projectId, setProjectId] = useState('')
  const [jobName, setJobName] = useState('')
  const { data: jobs, refetch } = useQuery(['syntheticJobs', projectId], () => syntheticApi.getJobs(projectId), { enabled: !!projectId })
  const mutation = useMutation(syntheticApi.createJob, { onSuccess: () => refetch() })

  const handleCreate = () => {
    mutation.mutate({
      projectId,
      jobName,
      startTime: '2024-01-01T00:00:00Z',
      endTime: '2024-01-02T00:00:00Z',
      equipmentCount: 2,
      scenarioConfig: { scenarios: ['normal', 'heavy_load'], noiseLevel: 0.02 },
    })
  }

  return (
    <div>
      <Typography variant="h4" gutterBottom>합성데이터 생성</Typography>
      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          <TextField label="Project ID" value={projectId} onChange={e => setProjectId(e.target.value)} />
          <TextField label="Job Name" value={jobName} onChange={e => setJobName(e.target.value)} />
          <Button variant="contained" onClick={handleCreate} disabled={mutation.isLoading}>생성</Button>
        </Box>
      </Paper>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Job Name</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Progress</TableCell>
              <TableCell>Created</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {jobs?.map((j) => (
              <TableRow key={j.id}>
                <TableCell>{j.jobName}</TableCell>
                <TableCell>{j.status}</TableCell>
                <TableCell>{j.progressPct}%</TableCell>
                <TableCell>{new Date(j.createdAt).toLocaleString()}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}
