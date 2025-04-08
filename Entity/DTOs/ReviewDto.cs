namespace Entity.DTOs
{
    public class ReviewDto
    {
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool DeleteAt { get; set; }
    }
}
