/*
Project: PBT Pro
Description: custom store for dotnet identity
Author: ismail
Date: December 2024
Version: 1.0
Additional Notes:
- this store to override default dotnet identity role logic

Changes Logs:
30/12/2024 - initial create
*/
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PBTPro.DAL.Store
{
    public class ApplicationRoleStore : IRoleStore<ApplicationRole>
    {
        private readonly PBTProDbContext _context;
        private bool disposedValue;

        public ApplicationRoleStore(PBTProDbContext context)
        {
            _context = context;
        }

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Add(role);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Update(role);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Remove(role);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (int.TryParse(roleId, out int id))
            {
                return _context.Roles.FindAsync(id).AsTask();
            }
            return Task.FromResult<ApplicationRole>(null);
        }

        public Task<ApplicationRole> FindByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return _context.Roles.FirstOrDefaultAsync(r => r.Name.ToUpper() == roleName.ToUpper(), cancellationToken);
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name.ToUpper());
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        // This method allows getting the role Id, which might be useful in some cases.
        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        // The method sets the RoleId, but it's not commonly needed for custom implementations of RoleStore.
        public Task SetRoleIdAsync(ApplicationRole role, string roleId, CancellationToken cancellationToken)
        {
            if (int.TryParse(roleId, out int id))
            {
                role.Id = id;
            }
            return Task.CompletedTask;
        }

        // Cleanup resources.
        public Task DisposeAsync()
        {
            // Optional, clean up any resources here if necessary.
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CustomRoleStore()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
