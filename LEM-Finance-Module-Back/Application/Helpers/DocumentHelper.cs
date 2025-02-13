using Application.Abstractions;
using Application.Models;
using Domain.Entities;

namespace Application.Helpers
{
    abstract class DocumentHelper
    {
        private readonly IApplicationDbContext _dbContext;

        public DocumentHelper(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DocumentDto>? GetDocumentsForDevice(int deviceId)
        {
            var deviceDocuments = _dbContext.Documents.Where(x => x.DeviceId == deviceId).ToList();
            if (!deviceDocuments.Any())
            {
                return null;
            }

            return GetDocumentsDto(deviceDocuments);

        }
        public List<DocumentDto>? GetDocumentsForModel(int modelId)
        {
            var modelDocuments = _dbContext.Documents.Where(x => x.ModelId == modelId).ToList();
            if (!modelDocuments.Any())
            {
                return null;
            }

            return GetDocumentsDto(modelDocuments);
        }

        private List<DocumentDto> GetDocumentsDto(List<Document> documents)
        {
            var result = new List<DocumentDto>();
            foreach (var document in documents)
            {
                var documentDto = new DocumentDto
                {
                    Name = document.Name,
                    Format = document.Format,
                };
                result.Add(documentDto);
            }

            return result;
        }
    }
}
