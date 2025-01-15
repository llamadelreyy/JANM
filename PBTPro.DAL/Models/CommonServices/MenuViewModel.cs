using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Models.CommonServices
{
    public class MenuViewItem
    {
        public string Text { get; set; }
        public string NavigateUrl { get; set; }
        public string IconUrl { get; set; }
        public int? BadgeText { get; set; }
        public List<MenuViewItem>? SubMenu { get; set; } = new List<MenuViewItem>();
    }
}
