import { useQuery } from 'react-query'
import { Link } from 'react-router-dom'
import { Typography, Button, Table, TableBody, TableCell, TableHead, TableRow, Paper } from '@mui/material'
import { projectApi } from '@/services/api'
import type { Project } from '@/types'

export default function ProjectListPage() {
  const { data, isLoading } = useQuery('projects', projectApi.getAll)

  if (isLoading) return <Typography>Loading...</Typography>

  return (
    <div>
      <Typography variant="h4" gutterBottom>프로젝트 목록</Typography>
      <Button variant="contained" sx={{ mb: 2 }}>새 프로젝트</Button>
      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>이름</TableCell>
              <TableCell>장비 유형</TableCell>
              <TableCell>상태</TableCell>
              <TableCell>생성일</TableCell>
              <TableCell>액션</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.map((p) => (
              <TableRow key={p.id}>
                <TableCell>{p.name}</TableCell>
                <TableCell>{p.equipmentType}</TableCell>
                <TableCell>{p.status}</TableCell>
                <TableCell>{new Date(p.createdAt).toLocaleDateString()}</TableCell>
                <TableCell>
                  <Button component={Link} to={`/projects/${p.id}`} size="small">상세</Button>
                  <Button component={Link} to={`/dashboard/${p.id}`} size="small">대시보드</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}
