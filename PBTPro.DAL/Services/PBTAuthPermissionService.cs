using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBTPro.DAL.Services
{
    using Microsoft.AspNetCore.Components;
    using PBTPro.DAL.Models.CommonServices;
    using System;
    using System.Linq;

    public class PBTAuthPermissionService
    {
        private readonly PBTAuthStateProvider _authStateProvider;

        private readonly NavigationManager _navigationManager;
        private AuthenticatedMenuPermission Permission { get; set; } = new AuthenticatedMenuPermission();

        public PBTAuthPermissionService(PBTAuthStateProvider authStateProvider, NavigationManager navigationManager)
        {
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }

        public async Task InitializePermissionAsync()
        {
            Permission = await LoadPermissionData();
        }

        private async Task<AuthenticatedMenuPermission> LoadPermissionData()
        {
            var menuPath = GetCurrentMenuPath();

            return Permission;
        }

        private string GetCurrentMenuPath()
        {
            var fullUrl = _navigationManager.Uri;
            var uri = new Uri(fullUrl);
            return uri.AbsolutePath.TrimStart('/');
        }

        public bool HasPermission(string action)
        {
            var menuPath = GetCurrentMenuPath();
            var permissions = _authStateProvider.Permissions;
            var menu = permissions.FirstOrDefault(p =>
                p.menu_path.Equals(menuPath, StringComparison.OrdinalIgnoreCase));

            return menu != null && action switch
            {
                "View" => menu.can_view,
                "Add" => menu.can_add,
                "Delete" => menu.can_delete,
                "Edit" => menu.can_edit,
                "Print" => menu.can_print,
                "Download" => menu.can_download,
                "Upload" => menu.can_upload,
                "Execute" => menu.can_execute,
                "Authorize" => menu.can_authorize,
                "ViewSensitive" => menu.can_view_sensitive,
                "ExportData" => menu.can_export_data,
                "ImportData" => menu.can_import_data,
                "ApproveChanges" => menu.can_approve_changes,
                _ => false
            };
        }
    }
}
