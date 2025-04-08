namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string code { get; set; }
        public DateTime CreatAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public RolUser RolUser { get; set; }
        public RolFormPermission RolFormPermission { get; set; }

    }
}
