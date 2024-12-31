using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.PayLoads
{
    public class permission_menu_view
    {
        public int menu_id { get; set; }
        public string menu_name { get; set; }
        public string menu_path { get; set; }
        public bool can_view { get; set; }
        public bool can_add { get; set; }
        public bool can_delete { get; set; }
        public bool can_edit { get; set; }
        public bool can_print { get; set; }
        public bool can_download { get; set; }
        public bool can_upload { get; set; }
        public bool can_execute { get; set; }
        public bool can_authorize { get; set; }
        public bool can_view_sensitive { get; set; }
        public bool can_export_data { get; set; }
        public bool can_import_data { get; set; }
        public bool can_approve_changes { get; set; }
    }
}
