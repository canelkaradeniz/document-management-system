import { useState, useEffect, useCallback } from "react";
import SearchBar from "./components/SearchBar";
import DocumentList from "./components/DocumentList";
import DocumentDetail from "./components/DocumentDetail";
import UploadForm from "./components/UploadForm";
import { searchDocuments } from "./services/api";
import type { DocumentListItem, DocumentType, SearchParams } from "./types";
import "./App.css";

export default function App() {
  const [documents, setDocuments] = useState<DocumentListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedDocId, setSelectedDocId] = useState<string | null>(null);
  const [showUpload, setShowUpload] = useState(false);
  const [searchParams, setSearchParams] = useState<SearchParams>({});

  const fetchDocuments = useCallback(async (params: SearchParams, pageNum: number) => {
    setLoading(true);
    setError(null);
    try {
      const result = await searchDocuments({ ...params, page: pageNum, pageSize: 20 });
      if (result.isSuccess && result.data) {
        setDocuments(result.data.items);
        setTotalCount(result.data.totalCount);
        setTotalPages(result.data.totalPages);
        setPage(result.data.page);
      } else {
        setError(result.error || "Dokümanlar yüklenemedi.");
      }
    } catch {
      setError("Sunucuya bağlanılamadı. Backend çalışıyor mu?");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchDocuments({}, 1);
  }, [fetchDocuments]);

  const handleSearch = (params: { search?: string; type?: DocumentType; uploadedBy?: string }) => {
    const newParams = { ...params };
    setSearchParams(newParams);
    fetchDocuments(newParams, 1);
  };

  const handlePageChange = (newPage: number) => {
    fetchDocuments(searchParams, newPage);
  };

  const handleUploaded = () => {
    fetchDocuments(searchParams, page);
  };

  if (selectedDocId) {
    return (
      <div className="app">
        <header>
          <h1>DocMan - Doküman Yönetim Sistemi</h1>
        </header>
        <main>
          <DocumentDetail
            documentId={selectedDocId}
            onBack={() => setSelectedDocId(null)}
          />
        </main>
      </div>
    );
  }

  return (
    <div className="app">
      <header>
        <div className="header-row">
          <h1>DocMan - Doküman Yönetim Sistemi</h1>
          <button onClick={() => setShowUpload(true)} className="btn btn-primary">
            + Doküman Yükle
          </button>
        </div>
      </header>

      <main>
        <SearchBar onSearch={handleSearch} />

        {loading && <div className="loading">Yükleniyor...</div>}

        {error && <div className="error-message">{error}</div>}

        {!loading && !error && (
          <DocumentList
            documents={documents}
            onSelect={setSelectedDocId}
            totalCount={totalCount}
            page={page}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        )}
      </main>

      {showUpload && (
        <UploadForm
          onClose={() => setShowUpload(false)}
          onUploaded={handleUploaded}
        />
      )}
    </div>
  );
}
