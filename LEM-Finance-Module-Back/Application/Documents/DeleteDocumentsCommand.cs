using Application.Abstractions.Messaging;

namespace Application.Documents
{
    public class DeleteDocumentsCommand(ICollection<int> documentsId) : ICommand<bool>
    {
        public ICollection<int> DocumentsId { get; set; } = documentsId;
    }
}
