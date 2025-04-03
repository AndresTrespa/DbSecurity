using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Active { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual ICollection<RolUser> RolUsers { get; set; }
    }
}
