namespace Entity.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool Active { get; set; }
        //public Persona Persona { get; set; }
        //public RolUser RolUsers { get; set; }
    }

}
