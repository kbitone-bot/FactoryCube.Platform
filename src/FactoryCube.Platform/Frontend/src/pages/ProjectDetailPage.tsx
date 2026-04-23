import { useParams, Link } from 'react-router-dom'
import { useQuery } from 'react-query'
import { Typography, Button, Paper, Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material'
import { projectApi, datasetApi } from '@/services/api'
import type { Project, Dataset } from '@/types'

export default function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: project } = useQuery(['project', id], () => projectApi.getById(id!), { enabled: !!id })
  const { data: datasets } = useQuery(['datasets', id], () => datasetApi.getByProject(id!), { enabled: !!id })

  return (
    <div>
      <Typography variant="h4" gutterBottom>{project?.name}</Typography>
      <Typography color="text.secondary" gutterBottom>{project?.description}</Typography>
      <Button variant="contained" sx={{ mb: 2 }} component={Link} to="/datasets/new">데이터셋 등록</Button>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>데이터셋명</TableCell>
              <TableCell>소스</TableCell>
              <TableCell>상태</TableCell>
              <TableCell>Quality</TableCell>
              <TableCell>액션</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {datasets?.map((d) => (
              <TableRow key={d.id}>
                <TableCell>{d.name}</TableCell>
                <TableCell>{d.sourceType}</TableCell>
                <TableCell>{d.status}</TableCell>
                <TableCell>{d.qualityScore ?? '-'}</TableCell>
                <TableCell>
                  <Button component={Link} to={`/datasets/${d.id}`} size="small">상세</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}
