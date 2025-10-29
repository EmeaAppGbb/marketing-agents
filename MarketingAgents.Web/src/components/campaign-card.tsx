import { Campaign } from '@/hooks/use-campaigns';

export interface CampaignCardProps {
  campaign: Campaign;
  onSelect?: (campaign: Campaign) => void;
}

export function CampaignCard({ campaign, onSelect }: CampaignCardProps) {
  const handleClick = () => {
    if (onSelect) {
      onSelect(campaign);
    }
  };

  const statusColors = {
    draft: 'bg-gray-100 text-gray-800',
    active: 'bg-green-100 text-green-800',
    completed: 'bg-blue-100 text-blue-800',
  };

  return (
    <div
      className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm transition-shadow hover:shadow-md"
      onClick={handleClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          handleClick();
        }
      }}
      data-testid="campaign-card"
    >
      <div className="mb-4 flex items-start justify-between">
        <h3 className="text-lg font-semibold text-gray-900">{campaign.title}</h3>
        <span
          className={`rounded-full px-3 py-1 text-xs font-medium ${statusColors[campaign.status]}`}
          data-testid="campaign-status"
        >
          {campaign.status}
        </span>
      </div>
      <p className="mb-4 text-sm text-gray-600">{campaign.description}</p>
      <div className="flex items-center justify-between text-xs text-gray-500">
        <span>Created: {new Date(campaign.createdAt).toLocaleDateString()}</span>
        <span>Updated: {new Date(campaign.updatedAt).toLocaleDateString()}</span>
      </div>
    </div>
  );
}
