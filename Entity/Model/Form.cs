﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public  class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public virtual ICollection<FormModule> FormModules { get; set; }
        public virtual ICollection<RolFormPermission> RolFormPermissions { get; set; }


    }
}
