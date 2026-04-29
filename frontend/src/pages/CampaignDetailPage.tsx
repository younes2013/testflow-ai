import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useCampaignStore } from '../store/campaignStore';
import { testCaseApi } from '../services/api';
import type { TestCase, TestCaseStatus } from '../types';

const PRIORITY_COLORS: Record<string, string> = {
  Low: '#6b7280',
  Medium: '#2563eb',
  High: '#d97706',
  Critical: '#dc2626',
};

export default function CampaignDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { selectedCampaign, loading, fetchById } = useCampaignStore();
  const [testCases, setTestCases] = useState<TestCase[]>([]);
  const [generating, setGenerating] = useState(false);
  const [aiPrompt, setAiPrompt] = useState('');

  useEffect(() => {
    if (id) {
      fetchById(Number(id));
      testCaseApi.getByCampaign(Number(id)).then(setTestCases);
    }
  }, [id, fetchById]);

  const handleGenerate = async () => {
    if (!aiPrompt.trim() || !id) return;
    setGenerating(true);
    try {
      const generated = await testCaseApi.generate(Number(id), {
        componentDescription: aiPrompt,
      });
      setTestCases(prev => [...prev, ...generated]);
      setAiPrompt('');
    } finally {
      setGenerating(false);
    }
  };

  const handleStatusChange = async (testCaseId: number, status: TestCaseStatus) => {
    const updated = await testCaseApi.updateStatus(testCaseId, status);
    setTestCases(prev => prev.map(t => t.id === testCaseId ? updated : t));
  };

  if (loading) return <div className="page-loading">Chargement...</div>;
  if (!selectedCampaign) return <div className="page-error">Campagne introuvable</div>;

  return (
    <div className="page">
      <div className="page-header">
        <button onClick={() => navigate('/campaigns')} className="btn btn-secondary">← Retour</button>
        <h1>{selectedCampaign.name}</h1>
      </div>

      <div className="campaign-meta">
        <span><strong>Système :</strong> {selectedCampaign.targetSystem}</span>
        <span><strong>Statut :</strong> {selectedCampaign.status}</span>
        <span><strong>Tests :</strong> {testCases.length}</span>
      </div>

      <div className="ai-section">
        <h2>Générer des cas de test avec l'IA</h2>
        <textarea
          value={aiPrompt}
          onChange={e => setAiPrompt(e.target.value)}
          placeholder="Décrivez le composant ou la fonctionnalité à tester (ex: module de communication CAN Bus, gestion des interruptions, watchdog timer...)"
          rows={4}
        />
        <button
          onClick={handleGenerate}
          disabled={generating || !aiPrompt.trim()}
          className="btn btn-ai"
        >
          {generating ? 'Génération en cours...' : 'Générer avec Ollama'}
        </button>
      </div>

      <div className="testcases-section">
        <h2>Cas de test ({testCases.length})</h2>
        {testCases.length === 0 ? (
          <p className="empty-state">Aucun cas de test. Utilisez la génération IA ou créez-en manuellement.</p>
        ) : (
          <div className="testcase-list">
            {testCases.map(tc => (
              <div key={tc.id} className={`testcase-card status-${tc.status.toLowerCase()}`}>
                <div className="testcase-header">
                  <div>
                    <span
                      className="priority-badge"
                      style={{ backgroundColor: PRIORITY_COLORS[tc.priority] }}
                    >
                      {tc.priority}
                    </span>
                    {tc.isAiGenerated && <span className="ai-badge">IA</span>}
                    <h3>{tc.title}</h3>
                  </div>
                  <select
                    value={tc.status}
                    onChange={e => handleStatusChange(tc.id, e.target.value as TestCaseStatus)}
                    className="status-select"
                  >
                    {['NotRun', 'InProgress', 'Passed', 'Failed', 'Blocked', 'Skipped'].map(s => (
                      <option key={s} value={s}>{s}</option>
                    ))}
                  </select>
                </div>
                <p>{tc.description}</p>
                {tc.riskScore > 0 && (
                  <span className="risk-score">Risque : {tc.riskScore.toFixed(1)}</span>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
