import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useQuery, useMutation } from 'react-query'
import { Typography, Button, Paper, Box } from '@mui/material'
import { datasetApi } from '@/services/api'
import type { Dataset } from '@/types'

export default function DatasetPage() {
  const { id } = useParams<{ id: string }>()
  const { data: dataset } = useQuery(['dataset', id], () => datasetApi.getById(id!), { enabled: !!id })
  const ingestMut = useMutation(() => datasetApi.ingest(id!))
  const [file, setFile] = useState<File | null>(null)
  const uploadMut = useMutation(() => datasetApi.upload(id!, file!), { onSuccess: () => window.location.reload() })

  return (
    <div>
      <Typography variant="h4" gutterBottom>데이터셋 상세</Typography>
      <Paper sx={{ p: 2, mb: 2 }}>
        <Typography>이름: {dataset?.name}</Typography>
        <Typography>상태: {dataset?.status}</Typography>
        <Typography>레코드 수: {dataset?.recordCount}</Typography>
        <Typography>Quality Score: {dataset?.qualityScore ?? '-'}</Typography>
        <Box sx={{ mt: 2, display: 'flex', gap: 2 }}>
          <Button variant="outlined" component="label">
            파일 선택
            <input type="file" hidden onChange={e => setFile(e.target.files?.[0] ?? null)} />
          </Button>
          <Button variant="contained" disabled={!file} onClick={() => uploadMut.mutate()}>업로드</Button>
          <Button variant="contained" onClick={() => ingestMut.mutate()}>적재 실행</Button>
          <Button variant="outlined" component={Link} to={`/quality/${id}`}>품질 검증</Button>
        </Box>
      </Paper>
    </div>
  )
}
