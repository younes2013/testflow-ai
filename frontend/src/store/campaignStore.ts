import { create } from 'zustand';
import { campaignApi } from '../services/api';
import type { Campaign, CreateCampaignRequest, UpdateCampaignRequest } from '../types';

interface CampaignState {
  campaigns: Campaign[];
  selectedCampaign: Campaign | null;
  loading: boolean;
  error: string | null;
  fetchAll: () => Promise<void>;
  fetchById: (id: number) => Promise<void>;
  create: (data: CreateCampaignRequest) => Promise<Campaign>;
  update: (id: number, data: UpdateCampaignRequest) => Promise<void>;
  remove: (id: number) => Promise<void>;
}

export const useCampaignStore = create<CampaignState>((set) => ({
  campaigns: [],
  selectedCampaign: null,
  loading: false,
  error: null,

  fetchAll: async () => {
    set({ loading: true, error: null });
    try {
      const campaigns = await campaignApi.getAll();
      set({ campaigns, loading: false });
    } catch (e) {
      set({ error: 'Erreur lors du chargement des campagnes', loading: false });
    }
  },

  fetchById: async (id) => {
    set({ loading: true, error: null });
    try {
      const campaign = await campaignApi.getById(id);
      set({ selectedCampaign: campaign, loading: false });
    } catch (e) {
      set({ error: 'Campagne introuvable', loading: false });
    }
  },

  create: async (data) => {
    const campaign = await campaignApi.create(data);
    set(state => ({ campaigns: [campaign, ...state.campaigns] }));
    return campaign;
  },

  update: async (id, data) => {
    const updated = await campaignApi.update(id, data);
    set(state => ({
      campaigns: state.campaigns.map(c => c.id === id ? updated : c),
      selectedCampaign: state.selectedCampaign?.id === id ? updated : state.selectedCampaign,
    }));
  },

  remove: async (id) => {
    await campaignApi.delete(id);
    set(state => ({
      campaigns: state.campaigns.filter(c => c.id !== id),
      selectedCampaign: state.selectedCampaign?.id === id ? null : state.selectedCampaign,
    }));
  },
}));
