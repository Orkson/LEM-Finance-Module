namespace TestLEM.Entities
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<MeasuredValue> MeasuredValues { get; set; }
    }
}