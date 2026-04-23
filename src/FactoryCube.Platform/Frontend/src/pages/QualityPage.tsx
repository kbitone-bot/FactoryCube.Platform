import { useParams } from 'react-router-dom'
import { useQuery, useMutation } from 'react-query'
import { Typography, Button, Paper, Table, TableBody, TableCell, TableHead, TableRow, LinearProgress, Box } from '@mui/material'
import { qualityApi } from '@/services/api'

export default function QualityPage() {
  const { datasetId } = useParams<{ datasetId: string }>()
  const { data, refetch } = useQuery(['quality', datasetId], () => qualityApi.getLatest(datasetId!), { enabled: !!datasetId })
  const checkMut = useMutation(() => qualityApi.runCheck(datasetId!), { onSuccess: () => refetch() })

  const score = data?.overallScore ?? 0
  const color = score >= 80 ? 'success' : score >= 50 ? 'warning' : 'error'

  return (
    <div>
      <Typography variant="h4" gutterBottom>데이터 품질 검증</Typography>
      <Button variant="contained" sx={{ mb: 2 }} onClick={() => checkMut.mutate()}>검증 실행</Button>
      <Paper sx={{ p: 2, mb: 2 }}>
        <Typography variant="h6">Overall Score: {score}</Typography>
        <LinearProgress variant="determinate" value={score} color={color} sx={{ height: 10, borderRadius: 1 }} />
        <Box sx={{ display: 'flex', gap: 4, mt: 2 }}>
          <Typography>Pass: {data?.passCount ?? 0}</Typography>
          <Typography>Warning: {data?.warningCount ?? 0}</Typography>
          <Typography>Reject: {data?.rejectCount ?? 0}</Typography>
        </Box>
      </Paper>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Level</TableCell>
              <TableCell>Issue Count</TableCell>
              <TableCell>Score</TableCell>
              <TableCell>Verdict</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.results?.map((r: any) => (
              <TableRow key={r.id}>
                <TableCell>{r.issueLevel}</TableCell>
                <TableCell>{r.issueCount}</TableCell>
                <TableCell>{r.score}</TableCell>
                <TableCell>{r.verdict}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}
