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
    
    public partial class tb_dwn_downloads
    {
        public int dwn_id { get; set; }
        public System.DateTime dwn_download_datetime { get; set; }
        public string dwn_link { get; set; }
        public int amz_id { get; set; }
    
        public virtual tb_amz_arquivos_armazenados tb_amz_arquivos_armazenados { get; set; }
    }
}
