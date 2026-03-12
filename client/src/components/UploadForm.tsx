import { useState } from "react";
import { uploadDocument, checkDuplicate } from "../services/api";
import { DocumentType, DocumentTypeLabels } from "../types";

interface Props {
  onClose: () => void;
  onUploaded: () => void;
}

// Basit bir hash hesaplama (gerçek projede SHA-256 kullanılır)
async function computeHash(file: File): Promise<string> {
  const buffer = await file.arrayBuffer();
  const hashBuffer = await crypto.subtle.digest("SHA-256", buffer);
  const hashArray = Array.from(new Uint8Array(hashBuffer));
  return hashArray.map((b) => b.toString(16).padStart(2, "0")).join("");
}

export default function UploadForm({ onClose, onUploaded }: Props) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [type, setType] = useState<DocumentType>(DocumentType.Contract);
  const [tagsInput, setTagsInput] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<{ text: string; type: "success" | "warning" | "error" } | null>(null);
  const [duplicateWarning, setDuplicateWarning] = useState<string | null>(null);
  const [forceUpload, setForceUpload] = useState(false);

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (!selectedFile) return;

    setFile(selectedFile);
    setDuplicateWarning(null);

    // Dosya seçildiğinde otomatik duplicate kontrolü
    try {
      const hash = await computeHash(selectedFile);
      const result = await checkDuplicate(hash);
      if (result.isDuplicate) {
        setDuplicateWarning(
          `Bu dosyanın aynısı zaten sistemde mevcut: "${result.existingDocumentName}". Yine de yüklemek istiyorsanız aşağıdaki kutuyu işaretleyin.`
        );
      }
    } catch {
      // Duplicate check başarısız olursa yüklemeye devam edebilsin
    }

    // Dosya adını otomatik doldur
    if (!name) {
      const nameWithoutExt = selectedFile.name.replace(/\.[^/.]+$/, "");
      setName(nameWithoutExt);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file) {
      setMessage({ text: "Lütfen bir dosya seçin.", type: "error" });
      return;
    }

    setLoading(true);
    setMessage(null);

    try {
      const hash = await computeHash(file);
      const tags = tagsInput
        .split(",")
        .map((t) => t.trim())
        .filter(Boolean);

      const result = await uploadDocument({
        name,
        description: description || undefined,
        type,
        fileExtension: file.name.substring(file.name.lastIndexOf(".")),
        fileSizeBytes: file.size,
        contentHash: hash,
        uploadedBy: "current-user", // Normalde auth'dan gelir
        tags,
        forceUpload,
      });

      if (result.isSuccess) {
        if (result.warnings && result.warnings.length > 0) {
          setMessage({ text: result.warnings.join(" "), type: "warning" });
        } else {
          setMessage({ text: "Doküman başarıyla yüklendi!", type: "success" });
          setTimeout(() => {
            onUploaded();
            onClose();
          }, 1500);
        }
      } else {
        setMessage({ text: result.error || "Yükleme sırasında bir hata oluştu.", type: "error" });
      }
    } catch {
      setMessage({ text: "Sunucuya bağlanılamadı.", type: "error" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Doküman Yükle</h2>
          <button onClick={onClose} className="close-btn">✕</button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Dosya</label>
            <input type="file" onChange={handleFileChange} />
          </div>

          {duplicateWarning && (
            <div className="warning-box">
              <p>{duplicateWarning}</p>
              <label className="checkbox-label">
                <input
                  type="checkbox"
                  checked={forceUpload}
                  onChange={(e) => setForceUpload(e.target.checked)}
                />
                Yine de yükle
              </label>
            </div>
          )}

          <div className="form-group">
            <label>Doküman Adı</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              maxLength={250}
            />
          </div>

          <div className="form-group">
            <label>Açıklama</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              maxLength={1000}
            />
          </div>

          <div className="form-group">
            <label>Doküman Tipi</label>
            <select
              value={type}
              onChange={(e) => setType(Number(e.target.value) as DocumentType)}
            >
              {Object.entries(DocumentTypeLabels).map(([val, label]) => (
                <option key={val} value={val}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Etiketler (virgülle ayırın)</label>
            <input
              type="text"
              value={tagsInput}
              onChange={(e) => setTagsInput(e.target.value)}
              placeholder="örn: fatura, 2024, abc-ltd"
            />
          </div>

          {message && (
            <div className={`message message-${message.type}`}>
              {message.text}
            </div>
          )}

          <div className="form-actions">
            <button type="button" onClick={onClose} className="btn btn-secondary">
              İptal
            </button>
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? "Yükleniyor..." : "Yükle"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
