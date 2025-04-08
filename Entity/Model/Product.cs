namespace Entity.Model
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int FavoriteId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}


