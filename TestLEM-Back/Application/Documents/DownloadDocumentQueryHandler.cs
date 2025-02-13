using Application.Models;
using Domain.Abstraction;
using MediatR;

namespace Application.Documents
{
    public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, FileResultDto>
    {
        private readonly IDocumentRepository _documentRepository;

        public DownloadDocumentQueryHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }


        public async Task<FileResultDto> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
        {
            var documents = await _documentRepository.GetDocumentsByName(request.DocoumentName);
            var documentToDownload = new Domain.Entities.Document();

            if (request.ModelId != null)
            {
                documentToDownload = documents.First(x => x.ModelId == request.ModelId);
            }

            if (request.DeviceId != null)
            {
                documentToDownload = documents.First(x => x.DeviceId == request.DeviceId);
            }

            var contentType = GetContentType(documentToDownload.Format);


            return new FileResultDto(documentToDownload.Data, contentType, documentToDownload.Name);
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".txt": return "text/plain";
                // Add more cases as needed
                default: return "application/octet-stream";
            }
        }
    }
}
