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
    
    public partial class Review
    {
        public int Id { get; set; }
        public int ConsumerId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    
        public virtual Consumer Consumer { get; set; }
        public virtual Product Product { get; set; }
    }
}
