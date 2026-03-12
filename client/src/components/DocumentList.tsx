import type { DocumentListItem } from "../types";

interface Props {
  documents: DocumentListItem[];
  onSelect: (id: string) => void;
  totalCount: number;
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

function getTypeIcon(type: number): string {
  switch (type) {
    case 1: return "📄";
    case 2: return "📋";
    case 3: return "🧾";
    default: return "📁";
  }
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

export default function DocumentList({
  documents,
  onSelect,
  totalCount,
  page,
  totalPages,
  onPageChange,
}: Props) {
  if (documents.length === 0) {
    return (
      <div className="empty-state">
        <p>Aramanızla eşleşen doküman bulunamadı.</p>
        <p className="hint">Farklı anahtar kelimeler veya filtreler deneyin.</p>
      </div>
    );
  }

  return (
    <div className="document-list">
      <div className="result-info">
        <span>{totalCount} doküman bulundu</span>
      </div>

      <table>
        <thead>
          <tr>
            <th>Tip</th>
            <th>Doküman Adı</th>
            <th>Uzantı</th>
            <th>Boyut</th>
            <th>Yükleyen</th>
            <th>Tarih</th>
            <th>Etiketler</th>
          </tr>
        </thead>
        <tbody>
          {documents.map((doc) => (
            <tr key={doc.id} onClick={() => onSelect(doc.id)} className="clickable-row">
              <td>{getTypeIcon(doc.type)} {doc.typeDisplayName}</td>
              <td className="doc-name">{doc.name}</td>
              <td>{doc.fileExtension}</td>
              <td>{doc.fileSizeDisplay}</td>
              <td>{doc.uploadedBy}</td>
              <td>{formatDate(doc.createdAt)}</td>
              <td>
                <div className="tags">
                  {doc.tags.map((tag) => (
                    <span key={tag} className="tag">{tag}</span>
                  ))}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {totalPages > 1 && (
        <div className="pagination">
          <button
            disabled={page <= 1}
            onClick={() => onPageChange(page - 1)}
            className="btn btn-small"
          >
            ← Önceki
          </button>
          <span>
            Sayfa {page} / {totalPages}
          </span>
          <button
            disabled={page >= totalPages}
            onClick={() => onPageChange(page + 1)}
            className="btn btn-small"
          >
            Sonraki →
          </button>
        </div>
      )}
    </div>
  );
}
