using TestLEM.Entities;

namespace TestLEM.Models
{
    public class ModelDto
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string CompanyName { get; set; }
        public ICollection<MeasuredValueDto> MeasuredValues { get; set; }
    }
}
