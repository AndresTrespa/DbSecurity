using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class FormModule
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int FormId { get; set; }

        public virtual Module Module { get; set; }
        public virtual Form Form { get; set; }
    }
}
