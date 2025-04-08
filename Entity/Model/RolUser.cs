namespace Entity.Model
{
    public class RolUser
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public int UserId { get; set; }

        public DateTime? DeleteAt { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual User User { get; set; }

    }
}
