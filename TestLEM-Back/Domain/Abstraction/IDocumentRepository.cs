using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IDocumentRepository
    {
        int AddDocument(Document document);
        Task<ICollection<string>> AddDocumentsAsync(ICollection<Document> documents, int? modelId, int? deviceId);
        Task<Document> GetFileByModelId(int modelId);
        Task<List<Document>> GetDocumentsByName(string documentName);
        Task RemoveDocumentsAsync(ICollection<int> documentIds);
        Task<ICollection<Document>> GetDocumentsByIds(ICollection<int> documentIds);
    }
}
