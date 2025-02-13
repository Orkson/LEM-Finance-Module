using Application.Abstractions.Messaging;
using Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Documents
{
    public class DeleteDocumentsCommandHandler : ICommandHandler<DeleteDocumentsCommand, bool>
    {
        private readonly IDocumentRepository _documentRepository;

        public DeleteDocumentsCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<bool> Handle(DeleteDocumentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _documentRepository.RemoveDocumentsAsync(request.DocumentsId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Usuniecie dokumentow nie powiodlo sie");
            }
        }
    }
}
