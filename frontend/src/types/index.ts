export type CampaignStatus = 'Draft' | 'InProgress' | 'Completed' | 'Archived';
export type TestCasePriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TestCaseStatus = 'NotRun' | 'InProgress' | 'Passed' | 'Failed' | 'Blocked' | 'Skipped';

export interface Campaign {
  id: number;
  name: string;
  description: string;
  targetSystem: string;
  status: CampaignStatus;
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
  testCases: TestCase[];
}

export interface TestCase {
  id: number;
  title: string;
  description: string;
  preconditions: string;
  steps: string;
  expectedResult: string;
  priority: TestCasePriority;
  status: TestCaseStatus;
  isAiGenerated: boolean;
  riskScore: number;
  createdAt: string;
  executedAt?: string;
  failureReason?: string;
  campaignId: number;
}

export interface CreateCampaignRequest {
  name: string;
  description: string;
  targetSystem: string;
}

export interface UpdateCampaignRequest extends CreateCampaignRequest {
  status: CampaignStatus;
}

export interface CreateTestCaseRequest {
  title: string;
  description: string;
  preconditions: string;
  steps: string;
  expectedResult: string;
  priority: TestCasePriority;
  campaignId: number;
}

export interface GenerateTestCasesRequest {
  componentDescription: string;
  model?: string;
}
