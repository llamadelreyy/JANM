using DevExpress.Blazor.Internal;
using DevExpress.CodeParser;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using System.Drawing.Drawing2D;

namespace PBTPro.Data
{
    public class FileMenuObject
    {
        public string Name { get; private set; }
        public bool? Checked { get; private set; } = false;
        public string MenuId { get; private set; }
        public string ParentId { get; private set; }
        public string PathNav { get; private set; }
        public bool IsMenu { get; private set; }
        public int SortOrder { get; private set; }
        public string IconUrl { get; private set; }
        public string IconCssClass { get; private set; }
        public string CssClass { get; private set; }
        public bool Selected { get; set; }

        public List<FileMenuObject> Content { get; private set; }
        public FileMenuObject(string name, string menuId, string parentId, string pathNav, bool boolMenu, int sortOrder, string iconUrl, string cssClass, List<FileMenuObject> content = null)
        {
            Name = name;
            MenuId = menuId;
            ParentId = parentId;
            PathNav = pathNav;
            IsMenu = boolMenu;
            SortOrder = sortOrder;
            IconUrl = iconUrl;
            IconCssClass = DefaultIcon(boolMenu, parentId, iconUrl);
            CssClass = cssClass;
            Selected = false;
            Content = content;
        }

        public FileMenuObject(string name, string menuId, string parentId, string pathNav, bool boolMenu, int sortOrder, string iconUrl, string cssClass, string iconCssClass, List<FileMenuObject> content = null)
        {
            Name = name;
            MenuId = menuId;
            ParentId = parentId;
            PathNav = pathNav;
            IsMenu = boolMenu;
            SortOrder = sortOrder;
            IconUrl = iconUrl;
            IconCssClass = iconCssClass;
            CssClass = cssClass;
            Selected = false;
            Content = content;
        }

        public string DefaultIcon(bool boolMenu, string parentId, string iconUrl)
        {
            if (parentId == "0")
            {
                if (!boolMenu)
                    return "treeview-icon-document";
            }
          
            return iconUrl;
        }
    }
}
