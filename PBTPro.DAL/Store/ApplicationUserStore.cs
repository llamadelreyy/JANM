/*
Project: PBT Pro
Description: custom store for dotnet identity
Author: ismail
Date: December 2024
Version: 1.0
Additional Notes:
- this store to override default dotnet identity user logic

Changes Logs:
30/12/2024 - initial create
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PBTPro.DAL.Models.PayLoads;

namespace PBTPro.DAL.Store
{
    public class ApplicationUserStore :
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>,
        IUserRoleStore<ApplicationUser>,
        //IUserClaimStore<ApplicationUser>,
        //IUserLoginStore<ApplicationUser>,
        //IUserTokenStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser>
    {
        private readonly PBTProDbContext _context;
        private readonly IdentityOptions _identityOptions;
        private bool disposedValue;

        public ApplicationUserStore(PBTProDbContext context, IOptions<IdentityOptions> identityOptions)
        {
            _context = context;
            _identityOptions = identityOptions.Value;
        }

        // IUserStore methods
        #region IUserStore Methods
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => IdentityResult.Success);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            int id = int.Parse(userId);
            return _context.Users.FindAsync(id).AsTask();
        }

        public Task<ApplicationUser> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == userName.ToUpper(), cancellationToken);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToUpper());
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
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
        // ~ApplicationUserStore()
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
        #endregion

        // IUserPasswordStore methods
        #region IUserPasswordStore Methods
        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }
        #endregion

        // IUserEmailStore methods
        #region IUserEmailStore Methods
        public Task<ApplicationUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper(), cancellationToken);
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToUpper());
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
        #endregion

        // IUserPhoneNumberStore methods
        #region IUserPhoneNumberStore Methods
        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion

        // IUserRoleStore methods
        #region IUserRoleStore Methods
        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                var userRole = new ApplicationUserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                _context.UserRoles.Add(userRole);
                return _context.SaveChangesAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                    return _context.SaveChangesAsync(cancellationToken);
                }
            }
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                return Task.FromResult<IList<string>>(new List<string>());
            }
            
            var userRolesTask = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync(cancellationToken);

            return userRolesTask.ContinueWith(userRoles =>
            {
                if (userRoles.Result == null || !userRoles.Result.Any())
                {
                    return (IList<string>)new List<string>();
                }

                var roles = userRoles.Result.Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name).ToList();
                return (IList<string>)roles;
            }, cancellationToken);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());
            if (role != null)
            {
                return Task.FromResult(_context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == role.Id));
            }
            return Task.FromResult(false);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());
            if (role == null)
            {
                return Task.FromResult<IList<ApplicationUser>>(new List<ApplicationUser>());
            }

            var userIds = _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToList();  // Synchronous call

            // Fetch the users based on the userIds
            var users = _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();  // Synchronous call

            return Task.FromResult((IList<ApplicationUser>)users);

        }
        #endregion

        // IUserClaimStore methods
        #region IUserClaimStore Methods
        /*
        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                _context.UserClaims.Add(new IdentityUserClaim<int>
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return _context.UserClaims
                .Where(uc => uc.UserId == user.Id)
                .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
                .ToListAsync(cancellationToken);
        }
        */
        #endregion

        // IUserLoginStore methods
        #region IUserLoginStore Methods
        /*
        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            _context.UserLogins.Add(new IdentityUserLogin<int>
            {
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            });
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var login = _context.UserLogins
                .FirstOrDefault(ul => ul.UserId == user.Id && ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey);
            if (login != null)
            {
                _context.UserLogins.Remove(login);
                return _context.SaveChangesAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var logins = _context.UserLogins
                .Where(ul => ul.UserId == user.Id)
                .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.LoginProvider))
                .ToListAsync(cancellationToken);
            return logins;
        }
        */
        #endregion

        // IUserTokenStore methods
        #region IUserTokenStore Methods
        /*
        public Task SetTokenAsync(ApplicationUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            var token = _context.UserTokens
                .FirstOrDefault(ut => ut.UserId == user.Id && ut.LoginProvider == loginProvider && ut.Name == name);

            if (token == null)
            {
                _context.UserTokens.Add(new IdentityUserToken<int>
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                });
            }
            else
            {
                token.Value = value;
            }
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<string> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var token = _context.UserTokens
                .FirstOrDefault(ut => ut.UserId == user.Id && ut.LoginProvider == loginProvider && ut.Name == name);
            return Task.FromResult(token?.Value);
        }

        public Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var token = _context.UserTokens
                .FirstOrDefault(ut => ut.UserId == user.Id && ut.LoginProvider == loginProvider && ut.Name == name);
            if (token != null)
            {
                _context.UserTokens.Remove(token);
                return _context.SaveChangesAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }
        */
        #endregion

        // IUserTwoFactorStore methods
        #region IUserTwoFactorStore Methods
        
        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }
        
        #endregion
        
        // IUserLockoutStore methods
        #region IUserLockoutStore Methods
        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetAccessFailedCountAsync(ApplicationUser user, int failedCount, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = failedCount;
            return Task.CompletedTask;
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;

            var maxFailedAttempts = _identityOptions.Lockout.MaxFailedAccessAttempts;
            var lockoutTimeSpan = _identityOptions.Lockout.DefaultLockoutTimeSpan;

            if (user.AccessFailedCount >= maxFailedAttempts)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.Add(lockoutTimeSpan);
                user.LockoutEnabled = true;
            }

            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
            user.LockoutEnabled = true;

            _context.Users.Update(user);
            return _context.SaveChangesAsync(cancellationToken).ContinueWith(t => user.AccessFailedCount);
        }

        #endregion

    }
}
