import axios from "axios";
import type {
  ApiResult,
  DocumentDetail,
  DocumentListItem,
  DuplicateCheckResult,
  PagedResult,
  SearchParams,
  UploadDocumentRequest,
} from "../types";

const api = axios.create({
  baseURL: "http://localhost:5218/api",
});

export async function searchDocuments(
  params: SearchParams
): Promise<ApiResult<PagedResult<DocumentListItem>>> {
  const { data } = await api.get("/documents", { params });
  return data;
}

export async function getDocumentById(
  id: string
): Promise<ApiResult<DocumentDetail>> {
  const { data } = await api.get(`/documents/${id}`);
  return data;
}

export async function uploadDocument(
  request: UploadDocumentRequest
): Promise<ApiResult<DocumentDetail>> {
  const { data } = await api.post("/documents", request);
  return data;
}

export async function checkDuplicate(
  contentHash: string
): Promise<DuplicateCheckResult> {
  const { data } = await api.get("/documents/check-duplicate", {
    params: { contentHash },
  });
  return data;
}

export async function getDocumentTypes(): Promise<
  { value: number; name: string; displayName: string }[]
> {
  const { data } = await api.get("/documents/types");
  return data;
}
