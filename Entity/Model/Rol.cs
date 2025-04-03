using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string code { get; set; }
        public DateTime CreatAt { get; set; }
        public bool DeleteAt { get; set; }
        public virtual ICollection<RolUser> RolUser { get; set; }
        public virtual ICollection<RolFormPermission> RolFormPermission { get; set; }

    }
}
