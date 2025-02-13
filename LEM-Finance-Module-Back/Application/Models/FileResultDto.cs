namespace Application.Models
{
    public class FileResultDto
    {
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }

        public FileResultDto(byte[] data, string contentType, string fileName)
        {
            Data = data;
            ContentType = contentType;
            FileName = fileName;
        }
    }
}
