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
    
    public partial class RegistriesHome
    {
        public int Id { get; set; }
        public string DocumentNr { get; set; }
        public string Explanations { get; set; }
        public Nullable<decimal> Income { get; set; }
        public Nullable<decimal> Outcome { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Id_apartment { get; set; }
        public Nullable<int> Id_RegHomeDaily { get; set; }
    
        public virtual Apartments Apartments { get; set; }
        public virtual RegistriesHomeDaily RegistriesHomeDaily { get; set; }
    }
}
