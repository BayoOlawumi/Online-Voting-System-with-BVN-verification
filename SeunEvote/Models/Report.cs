//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SeunEvote.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Report
    {
        public int VoterId { get; set; }
        public Nullable<int> President { get; set; }
        public Nullable<int> VicePresident { get; set; }
        public Nullable<int> FinancialSecretary { get; set; }
        public int VotedId { get; set; }
        public Nullable<int> Treasurer { get; set; }
    }
}
