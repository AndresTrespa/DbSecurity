namespace Entity.Model
{
    public class Consumer
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public bool Active { get; set; }
        //public ICollection<Favorite> Favorite { get; set; }
        //public ICollection<Review> Review { get; set; }
        //public ICollection<Order> Order { get; set; }
    }
}
