using Application.Abstractions.Messaging;
using Application.Models;

namespace Application.Documents
{
    public class DownloadDocumentQuery(string? documentName, int? deviceId, int? modelId) : IQuery<FileResultDto>
    {
        public int? DeviceId { get; set; } = deviceId;
        public int? ModelId { get; set; } = modelId;
        public string? DocoumentName { get; set; } = documentName;
    }
}
