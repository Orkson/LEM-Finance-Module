namespace Domain.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public byte[] Data { get; set; }
        public DateTime? DateAdded { get; set; }

        public int? DeviceId { get; set; }
        public int? ModelId { get; set; }

        public virtual Device Device { get; set; }
        public virtual Model Model { get; set; }
    }
}
