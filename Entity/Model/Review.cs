namespace Entity.Model
{
    public class Review
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool DeleteAt { get; set; }
    }
}
