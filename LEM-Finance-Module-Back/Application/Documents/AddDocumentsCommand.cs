using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace Application.Documents
{
    public class AddDocumentsCommand(ICollection<IFormFile> documentsDto, int? modelId, int? deviceId) : ICommand<ICollection<string>>
    {
        public ICollection<IFormFile> Documents { get; set; } = documentsDto;
        public int? ModelId { get; set; } = modelId;
        public int? DeviceId { get; set;} = deviceId;
    }
}
