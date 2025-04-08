namespace Entity.Model
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RolFormPermission RolFormPermission { get; set; }

    }
}
