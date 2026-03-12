export enum DocumentType {
  Contract = 1,
  Proposal = 2,
  Invoice = 3,
}

export const DocumentTypeLabels: Record<DocumentType, string> = {
  [DocumentType.Contract]: "Sözleşme",
  [DocumentType.Proposal]: "Teklif",
  [DocumentType.Invoice]: "Fatura",
};

export interface DocumentListItem {
  id: string;
  name: string;
  type: DocumentType;
  typeDisplayName: string;
  fileExtension: string;
  fileSizeDisplay: string;
  uploadedBy: string;
  createdAt: string;
  tags: string[];
}

export interface DocumentDetail {
  id: string;
  name: string;
  description: string | null;
  type: DocumentType;
  typeDisplayName: string;
  fileExtension: string;
  fileSizeBytes: number;
  fileSizeDisplay: string;
  uploadedBy: string;
  createdAt: string;
  updatedAt: string | null;
  tags: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiResult<T> {
  isSuccess: boolean;
  data: T | null;
  error: string | null;
  warnings: string[];
}

export interface SearchParams {
  search?: string;
  type?: DocumentType;
  uploadedBy?: string;
  dateFrom?: string;
  dateTo?: string;
  tag?: string;
  sortBy?: string;
  sortDesc?: boolean;
  page?: number;
  pageSize?: number;
}

export interface UploadDocumentRequest {
  name: string;
  description?: string;
  type: DocumentType;
  fileExtension: string;
  fileSizeBytes: number;
  contentHash: string;
  uploadedBy: string;
  tags: string[];
  forceUpload?: boolean;
}

export interface DuplicateCheckResult {
  isDuplicate: boolean;
  existingDocumentId: string | null;
  existingDocumentName: string | null;
  message: string;
}
