//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IronNajaUiPathActivities
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_fky_found_keys
    {
        public int fky_id { get; set; }
        public int amz_id { get; set; }
        public string fky_keys { get; set; }
    
        public virtual tb_amz_arquivos_armazenados tb_amz_arquivos_armazenados { get; set; }
    }
}