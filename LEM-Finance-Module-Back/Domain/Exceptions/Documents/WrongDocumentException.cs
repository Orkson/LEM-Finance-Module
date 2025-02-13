namespace Domain.Exceptions.Documents
{
    public sealed class WrongDocumentException : Exception
    {
        public WrongDocumentException(string documentName) : 
            base($"Document {documentName} can not be added to database!")
        {
        }
    }
}
