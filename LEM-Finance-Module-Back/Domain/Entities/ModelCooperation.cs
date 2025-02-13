namespace Domain.Entities
{
    public class ModelCooperation
    {
        public int Id { get; set; }
        public int ModelFromId { get; set; }
        public virtual Model ModelFrom { get; set; }

        public int ModelToId { get; set; }
        public virtual Model ModelTo { get; set; }
    }
}
