import { useState } from "react";
import { DocumentType, DocumentTypeLabels } from "../types";

interface Props {
  onSearch: (params: {
    search?: string;
    type?: DocumentType;
    uploadedBy?: string;
  }) => void;
}

export default function SearchBar({ onSearch }: Props) {
  const [search, setSearch] = useState("");
  const [type, setType] = useState<DocumentType | "">("");
  const [uploadedBy, setUploadedBy] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch({
      search: search || undefined,
      type: type || undefined,
      uploadedBy: uploadedBy || undefined,
    });
  };

  const handleClear = () => {
    setSearch("");
    setType("");
    setUploadedBy("");
    onSearch({});
  };

  return (
    <form onSubmit={handleSubmit} className="search-bar">
      <div className="search-row">
        <input
          type="text"
          placeholder="Doküman ara (isim veya açıklama)..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="search-input"
        />
        <select
          value={type}
          onChange={(e) =>
            setType(e.target.value ? (Number(e.target.value) as DocumentType) : "")
          }
          className="search-select"
        >
          <option value="">Tüm Tipler</option>
          {Object.entries(DocumentTypeLabels).map(([val, label]) => (
            <option key={val} value={val}>
              {label}
            </option>
          ))}
        </select>
        <input
          type="text"
          placeholder="Yükleyen kullanıcı..."
          value={uploadedBy}
          onChange={(e) => setUploadedBy(e.target.value)}
          className="search-input-small"
        />
        <button type="submit" className="btn btn-primary">
          Ara
        </button>
        <button type="button" onClick={handleClear} className="btn btn-secondary">
          Temizle
        </button>
      </div>
    </form>
  );
}
