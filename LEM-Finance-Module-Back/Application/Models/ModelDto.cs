using Microsoft.AspNetCore.Http;

namespace Application.Models
{
    public class ModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string? CompanyName { get; set; }
        public ICollection<MeasuredValueDto>? MeasuredValues { get; set; }
        public ICollection<int>? CooperatedModelsIds { get; set; }
    }
}
