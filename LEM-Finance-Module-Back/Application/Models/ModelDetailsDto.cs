namespace Application.Models
{
    public class ModelDetailsDto : ModelDto
    {
        public int TotalModelCount { get; set; }
        public ICollection<ModelDetailsDto>? RelatedModelsDetails { get; set; }
        public ICollection<DocumentDto>? ModelDocuments { get; set; }
        public int MyProperty { get; set; }
    }
}
