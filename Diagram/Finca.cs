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
    
    public partial class Finca
    {
        public int Id { get; set; }
        public int ProducerId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public short Hectares { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual Producer Producer { get; set; }
    }
}
