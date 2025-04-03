using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolFormPermission
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual Form Form { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
