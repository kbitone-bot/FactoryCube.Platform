import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import ProjectListPage from './pages/ProjectListPage'
import ProjectDetailPage from './pages/ProjectDetailPage'
import DatasetPage from './pages/DatasetPage'
import SyntheticPage from './pages/SyntheticPage'
import QualityPage from './pages/QualityPage'
import MlPage from './pages/MlPage'
import DashboardPage from './pages/DashboardPage'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<ProjectListPage />} />
        <Route path="/projects/:id" element={<ProjectDetailPage />} />
        <Route path="/datasets/:id" element={<DatasetPage />} />
        <Route path="/synthetic" element={<SyntheticPage />} />
        <Route path="/quality/:datasetId" element={<QualityPage />} />
        <Route path="/ml" element={<MlPage />} />
        <Route path="/dashboard/:projectId" element={<DashboardPage />} />
      </Routes>
    </Layout>
  )
}

export default App
