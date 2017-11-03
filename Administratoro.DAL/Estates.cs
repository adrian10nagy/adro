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
    
    public partial class Estates
    {
        public Estates()
        {
            this.EstateExpenses = new HashSet<EstateExpenses>();
            this.StairCases = new HashSet<StairCases>();
            this.Tenants = new HashSet<Tenants>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Id_Partner { get; set; }
        public Nullable<decimal> Indiviza { get; set; }
        public bool HasStaircase { get; set; }
    
        public virtual ICollection<EstateExpenses> EstateExpenses { get; set; }
        public virtual Partners Partners { get; set; }
        public virtual ICollection<StairCases> StairCases { get; set; }
        public virtual ICollection<Tenants> Tenants { get; set; }
    }
}