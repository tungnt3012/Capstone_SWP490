//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Capstone_SWP490
{
    using System;
    using System.Collections.Generic;
    
    public partial class school
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public school()
        {
            this.teams = new HashSet<team>();
        }
    
        public int school_id { get; set; }
        public string school_name { get; set; }
        public string institution_name { get; set; }
        public string address { get; set; }
        public string insert_date { get; set; }
        public Nullable<int> active { get; set; }
        public string update_date { get; set; }
        public Nullable<int> coach_id { get; set; }
        public Nullable<bool> enabled { get; set; }
        public string rector_name { get; set; }
        public string website { get; set; }
        public string phone_number { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<team> teams { get; set; }
    }
}
