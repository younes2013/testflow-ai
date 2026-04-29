import axios from 'axios';
import type {
  Campaign,
  CreateCampaignRequest,
  UpdateCampaignRequest,
  TestCase,
  CreateTestCaseRequest,
  GenerateTestCasesRequest,
  TestCaseStatus,
} from '../types';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api',
  headers: { 'Content-Type': 'application/json' },
});

export const campaignApi = {
  getAll: () => api.get<Campaign[]>('/campaigns').then(r => r.data),
  getById: (id: number) => api.get<Campaign>(`/campaigns/${id}`).then(r => r.data),
  create: (data: CreateCampaignRequest) => api.post<Campaign>('/campaigns', data).then(r => r.data),
  update: (id: number, data: UpdateCampaignRequest) =>
    api.put<Campaign>(`/campaigns/${id}`, data).then(r => r.data),
  delete: (id: number) => api.delete(`/campaigns/${id}`),
};

export const testCaseApi = {
  getByCampaign: (campaignId: number) =>
    api.get<TestCase[]>(`/testcases/campaign/${campaignId}`).then(r => r.data),
  getById: (id: number) => api.get<TestCase>(`/testcases/${id}`).then(r => r.data),
  create: (data: CreateTestCaseRequest) => api.post<TestCase>('/testcases', data).then(r => r.data),
  updateStatus: (id: number, status: TestCaseStatus, failureReason?: string) =>
    api.patch<TestCase>(`/testcases/${id}/status`, { status, failureReason }).then(r => r.data),
  generate: (campaignId: number, data: GenerateTestCasesRequest) =>
    api.post<TestCase[]>(`/testcases/campaign/${campaignId}/generate`, data).then(r => r.data),
};
