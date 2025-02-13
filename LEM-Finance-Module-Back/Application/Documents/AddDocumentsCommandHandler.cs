using Application.Abstractions.Messaging;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Exceptions.Documents;
using MediatR;

namespace Application.Documents
{
    internal class AddDocumentsCommandHandler : ICommandHandler<AddDocumentsCommand, ICollection<string>>
    {
        private readonly IDocumentRepository _documentRepository;

        public AddDocumentsCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<ICollection<string>> Handle(AddDocumentsCommand request, CancellationToken cancellationToken)
        {
            var documents = new List<Document>();

            foreach (var file in request.Documents)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var data = memoryStream.ToArray();
                        var fileName = file.FileName;
                        var fileFormat = Path.GetExtension(fileName);

                        var document = new Document
                        {
                            Name = fileName,
                            Format = fileFormat,
                            Data = data,
                            DateAdded = DateTime.Now
                        };
                        documents.Add(document);
                    }
                } 
                else
                {
                    throw new WrongDocumentException(file.Name);
                }
            }
            return await _documentRepository.AddDocumentsAsync(documents, request.ModelId, request.DeviceId);
        }
    }
}
