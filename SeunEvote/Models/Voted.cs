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
    
    public partial class Voted
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public bool HasVoted { get; set; }
        public string Encoded { get; set; }
        public string BVN { get; set; }
        public string PHONE_NUMBER { get; set; }
    }
}
