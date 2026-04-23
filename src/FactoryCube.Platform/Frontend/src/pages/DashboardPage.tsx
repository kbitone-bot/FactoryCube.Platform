import { useQuery } from 'react-query'
import { useParams } from 'react-router-dom'
import { Typography, Grid, Paper, Box } from '@mui/material'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts'
import { dashboardApi } from '@/services/api'

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8']

export default function DashboardPage() {
  const { projectId } = useParams<{ projectId: string }>()
  const { data: snapshot } = useQuery(['dashboard', projectId], () => dashboardApi.getSnapshot(projectId!), { enabled: !!projectId })
  const { data: eqStatus } = useQuery(['eqStatus', projectId], () => dashboardApi.getEquipmentStatus(projectId!), { enabled: !!projectId })

  const stateData = eqStatus
    ? Object.entries(
        eqStatus.reduce((acc: Record<string, number>, cur: any) => {
          acc[cur.currentState] = (acc[cur.currentState] || 0) + 1
          return acc
        }, {})
      ).map(([name, value]) => ({ name, value }))
    : []

  return (
    <div>
      <Typography variant="h4" gutterBottom>운영 대시보드</Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} md={3}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="subtitle2" color="text.secondary">가동률</Typography>
            <Typography variant="h3">{snapshot?.kpis?.availabilityPct ?? '--'}%</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={3}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="subtitle2" color="text.secondary">Health Score 평균</Typography>
            <Typography variant="h3">{snapshot?.kpis?.avgHealth ?? '--'}</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={3}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="subtitle2" color="text.secondary">알람 수</Typography>
            <Typography variant="h3">{snapshot?.kpis?.alarmCount ?? '--'}</Typography>
          </Paper>
        </Grid>
        <Grid item xs={12} md={3}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="subtitle2" color="text.secondary">Cycle Time 평균</Typography>
            <Typography variant="h3">{snapshot?.kpis?.avgCycleTime ?? '--'}s</Typography>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>장비 상태 분포</Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie data={stateData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={100} label>
                  {stateData.map((_, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>Health Score 랭킹</Typography>
            <Box>
              {eqStatus?.map((eq: any) => (
                <Box key={eq.equipmentId} sx={{ display: 'flex', justifyContent: 'space-between', py: 1 }}>
                  <Typography>{eq.equipmentId}</Typography>
                  <Typography color={eq.healthScore < 50 ? 'error' : 'success'}>{eq.healthScore}</Typography>
                </Box>
              ))}
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </div>
  )
}
