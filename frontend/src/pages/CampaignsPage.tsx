import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useCampaignStore } from '../store/campaignStore';
import type { CampaignStatus } from '../types';

const STATUS_LABELS: Record<CampaignStatus, string> = {
  Draft: 'Brouillon',
  InProgress: 'En cours',
  Completed: 'Terminée',
  Archived: 'Archivée',
};

const STATUS_COLORS: Record<CampaignStatus, string> = {
  Draft: '#6b7280',
  InProgress: '#2563eb',
  Completed: '#16a34a',
  Archived: '#9ca3af',
};

export default function CampaignsPage() {
  const { campaigns, loading, error, fetchAll } = useCampaignStore();

  useEffect(() => { fetchAll(); }, [fetchAll]);

  if (loading) return <div className="page-loading">Chargement des campagnes...</div>;
  if (error) return <div className="page-error">{error}</div>;

  return (
    <div className="page">
      <div className="page-header">
        <h1>Campagnes de tests</h1>
        <Link to="/campaigns/new" className="btn btn-primary">+ Nouvelle campagne</Link>
      </div>

      {campaigns.length === 0 ? (
        <div className="empty-state">
          <p>Aucune campagne. Créez-en une pour commencer.</p>
        </div>
      ) : (
        <div className="campaign-grid">
          {campaigns.map(campaign => {
            const total = campaign.testCases.length;
            const passed = campaign.testCases.filter(t => t.status === 'Passed').length;
            const failed = campaign.testCases.filter(t => t.status === 'Failed').length;
            const progress = total > 0 ? Math.round((passed / total) * 100) : 0;

            return (
              <Link to={`/campaigns/${campaign.id}`} key={campaign.id} className="campaign-card">
                <div className="campaign-card-header">
                  <h2>{campaign.name}</h2>
                  <span
                    className="status-badge"
                    style={{ backgroundColor: STATUS_COLORS[campaign.status] }}
                  >
                    {STATUS_LABELS[campaign.status]}
                  </span>
                </div>
                <p className="campaign-system">{campaign.targetSystem}</p>
                <p className="campaign-description">{campaign.description}</p>
                <div className="campaign-stats">
                  <span>{total} tests</span>
                  <span className="stat-passed">{passed} réussis</span>
                  <span className="stat-failed">{failed} échecs</span>
                </div>
                {total > 0 && (
                  <div className="progress-bar">
                    <div className="progress-fill" style={{ width: `${progress}%` }} />
                  </div>
                )}
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}
