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
    
    public partial class PartnerRights
    {
        public int Id { get; set; }
        public int Id_user { get; set; }
        public int Id_functionality { get; set; }
    
        public virtual Functionalities Functionalities { get; set; }
        public virtual Partners Partners { get; set; }
    }
}
