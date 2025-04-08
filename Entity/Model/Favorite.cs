using System;

namespace Entity.Model
{
    public class Favorite    
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int ProducerId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date_Added { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
