import { useEffect, useState } from "react";
import { getDocumentById } from "../services/api";
import type { DocumentDetail as DocumentDetailType } from "../types";

interface Props {
  documentId: string;
  onBack: () => void;
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function DocumentDetail({ documentId, onBack }: Props) {
  const [document, setDocument] = useState<DocumentDetailType | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDocument = async () => {
      try {
        setLoading(true);
        setDocument(null);
        setError(null);
        const result = await getDocumentById(documentId);
        if (result.isSuccess && result.data) {
          setDocument(result.data);
        } else {
          setError(result.error || "Doküman yüklenemedi.");
        }
      } catch {
        setError("Sunucuya bağlanılamadı.");
      } finally {
        setLoading(false);
      }
    };
    fetchDocument();
  }, [documentId]);

  if (loading) return <div className="loading">Yükleniyor...</div>;
  if (error) return <div className="error-message">{error}</div>;
  if (!document) return null;

  return (
    <div className="document-detail">
      <button onClick={onBack} className="btn btn-secondary back-btn">
        ← Listeye Dön
      </button>

      <div className="detail-card">
        <h2>{document.name}</h2>

        <div className="detail-grid">
          <div className="detail-item">
            <label>Tip</label>
            <span>{document.typeDisplayName}</span>
          </div>
          <div className="detail-item">
            <label>Dosya Uzantısı</label>
            <span>{document.fileExtension}</span>
          </div>
          <div className="detail-item">
            <label>Boyut</label>
            <span>{document.fileSizeDisplay}</span>
          </div>
          <div className="detail-item">
            <label>Yükleyen</label>
            <span>{document.uploadedBy}</span>
          </div>
          <div className="detail-item">
            <label>Oluşturulma Tarihi</label>
            <span>{formatDate(document.createdAt)}</span>
          </div>
          {document.updatedAt && (
            <div className="detail-item">
              <label>Güncellenme Tarihi</label>
              <span>{formatDate(document.updatedAt)}</span>
            </div>
          )}
        </div>

        {document.description && (
          <div className="detail-description">
            <label>Açıklama</label>
            <p>{document.description}</p>
          </div>
        )}

        {document.tags.length > 0 && (
          <div className="detail-tags">
            <label>Etiketler</label>
            <div className="tags">
              {document.tags.map((tag) => (
                <span key={tag} className="tag">{tag}</span>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
