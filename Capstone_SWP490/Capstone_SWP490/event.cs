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
    
    public partial class @event
    {
        public int event_id { get; set; }
        public short event_type { get; set; }
        public string title { get; set; }
        public string desctiption { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string venue { get; set; }
        public string contactor_name { get; set; }
        public string contactor_email { get; set; }
        public string contactor_phone { get; set; }
        public string fan_page { get; set; }
        public Nullable<short> shirt_id { get; set; }
        public string note { get; set; }
    }
}