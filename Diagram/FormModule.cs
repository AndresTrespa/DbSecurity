//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Diagram
{
    using System;
    using System.Collections.Generic;
    
    public partial class FormModule
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int FormId { get; set; }
    
        public virtual Module Module { get; set; }
        public virtual Form Form { get; set; }
    }
}
