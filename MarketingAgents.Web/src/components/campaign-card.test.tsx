import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { CampaignCard } from './campaign-card';
import { Campaign } from '@/hooks/use-campaigns';

describe('CampaignCard', () => {
  const mockCampaign: Campaign = {
    id: '1',
    title: 'Summer Sale Campaign',
    description: 'A campaign for summer sales',
    status: 'active',
    createdAt: '2025-01-01T00:00:00Z',
    updatedAt: '2025-01-15T00:00:00Z',
  };

  it('renders campaign information correctly', () => {
    render(<CampaignCard campaign={mockCampaign} />);

    expect(screen.getByText('Summer Sale Campaign')).toBeInTheDocument();
    expect(screen.getByText('A campaign for summer sales')).toBeInTheDocument();
    expect(screen.getByTestId('campaign-status')).toHaveTextContent('active');
  });

  it('displays correct status color', () => {
    const { rerender } = render(<CampaignCard campaign={mockCampaign} />);
    expect(screen.getByTestId('campaign-status')).toHaveClass('bg-green-100');

    const draftCampaign = { ...mockCampaign, status: 'draft' as const };
    rerender(<CampaignCard campaign={draftCampaign} />);
    expect(screen.getByTestId('campaign-status')).toHaveClass('bg-gray-100');

    const completedCampaign = { ...mockCampaign, status: 'completed' as const };
    rerender(<CampaignCard campaign={completedCampaign} />);
    expect(screen.getByTestId('campaign-status')).toHaveClass('bg-blue-100');
  });

  it('calls onSelect when clicked', async () => {
    const onSelect = vi.fn();
    const user = userEvent.setup();

    render(<CampaignCard campaign={mockCampaign} onSelect={onSelect} />);

    const card = screen.getByTestId('campaign-card');
    await user.click(card);

    expect(onSelect).toHaveBeenCalledWith(mockCampaign);
    expect(onSelect).toHaveBeenCalledTimes(1);
  });

  it('calls onSelect when Enter key is pressed', async () => {
    const onSelect = vi.fn();
    const user = userEvent.setup();

    render(<CampaignCard campaign={mockCampaign} onSelect={onSelect} />);

    const card = screen.getByTestId('campaign-card');
    card.focus();
    await user.keyboard('{Enter}');

    expect(onSelect).toHaveBeenCalledWith(mockCampaign);
  });

  it('does not throw error when onSelect is not provided', async () => {
    const user = userEvent.setup();

    render(<CampaignCard campaign={mockCampaign} />);

    const card = screen.getByTestId('campaign-card');
    await expect(user.click(card)).resolves.not.toThrow();
  });

  it('formats dates correctly', () => {
    render(<CampaignCard campaign={mockCampaign} />);

    expect(screen.getByText(/Created:/)).toBeInTheDocument();
    expect(screen.getByText(/Updated:/)).toBeInTheDocument();
  });
});
