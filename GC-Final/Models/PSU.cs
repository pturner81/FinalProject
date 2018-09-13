//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GC_Final.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PSU
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PSU()
        {
            this.Builds = new HashSet<Build>();
        }
    
        public string PSUID { get; set; }
        public string product_id { get; set; }
        public string power_source { get; set; }
        public string product_dimensions { get; set; }
        public int wattage { get; set; }
        public string title { get; set; }
        public string product_description { get; set; }
        public string brand { get; set; }
        public int price { get; set; }
        public string stars { get; set; }
        public byte[] main_image { get; set; }
        public string manufacturer { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Build> Builds { get; set; }
    }
}
