//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Administratoro.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class PartnerTypes
    {
        public PartnerTypes()
        {
            this.Partners = new HashSet<Partners>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Partners> Partners { get; set; }
    }
}