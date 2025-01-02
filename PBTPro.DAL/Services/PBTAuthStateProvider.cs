using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using PBTPro.DAL.Models.PayLoads;
using System.Security.Claims;

namespace PBTPro.DAL.Services;

public class PBTAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly PBTAuthUserService _PBTAuthUserService;
    public AuthenticatedUser CurrentUser { get; private set; } = new();
    public string strUserFullNameNRole { get; set; } = "";
    public string accessToken { get; set; } = "";
    public List<AuthenticatedMenuPermission> Permissions { get; private set; } = new List<AuthenticatedMenuPermission>();

    public PBTAuthStateProvider(PBTAuthUserService PBTAuthUserService)
    {
        _PBTAuthUserService = PBTAuthUserService;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }

    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;

        if (authenticationState is not null)
        {
            CurrentUser = AuthenticatedUser.FromClaimsPrincipal(authenticationState.User);
            if (CurrentUser.Userid == null)
                await GetAuthenticationStateAsync();
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();
        var user = await _PBTAuthUserService.FetchUserFromBrowserAsync();

        if (user is not null)
        {
            //var userInDatabase = _PBTAuthUserService.LookupUserFromAPIAsync(user.Username, user.Password);
            /*
            if (userInDatabase is not null)
            {
                principal = userInDatabase.ToClaimsPrincipal();
                CurrentUser = userInDatabase;
                strUserFullNameNRole = user.Fullname + " (" + user.Roles + ")";
            }
            */
            CurrentUser = user;
            strUserFullNameNRole = user.Fullname + " (" + user.Roles + ")";
            accessToken = user.Token;
            principal = user.ToClaimsPrincipal();

            var permissionResponse = await _PBTAuthUserService.LookupPermissionFromAPIAsync(accessToken);
            if (permissionResponse.ReturnCode == 200)
            {
                string? permissionDataString = permissionResponse?.Data?.ToString();
                if (!string.IsNullOrWhiteSpace(permissionDataString))
                {
                    var test = JsonConvert.DeserializeObject<List<AuthenticatedMenuPermission>>(permissionDataString);
                    Permissions = JsonConvert.DeserializeObject<List<AuthenticatedMenuPermission>>(permissionDataString);
                }
            }
        }
        return new(principal);
    }

    public async Task<ReturnViewModel> LoginAsync(string username, string password)
    {
        var result = new ReturnViewModel();
        try
        {
            //var principal = new ClaimsPrincipal();
            //var response = await _PBTAuthUserService.LookupUserFromAPIAsync(username, password);
            //result = response;

            //if (response.ReturnCode == 200)
            //{
            //    string? dataString = response?.Data?.ToString();
            //    if (!string.IsNullOrWhiteSpace(dataString))
            //    {
            //        var data = JsonConvert.DeserializeObject<LoginResult>(dataString);

            //        var user = new AuthenticatedUser
            //        {
            //            Fullname = data.Fullname,
            //            Token = data.Token,
            //            Userid = data.Userid,
            //            Username = data.Username,
            //            Roles = data.Roles,
            //            Password = password
            //        };

            //        strUserFullNameNRole = user.Fullname + " (" + user.Roles + ")";
            //        accessToken = user.Token ?? string.Empty;
            //        await _PBTAuthUserService.PersistUserToBrowserAsync(user);
            //        principal = user.ToClaimsPrincipal();
            //    }
            //}

            // ================= For Testing purpose ======================
            var principal = new ClaimsPrincipal();
            var response = await _PBTAuthUserService.LookupUserFromAPIAsync(username, password);
            result = response;

            var user = new AuthenticatedUser
            {
                Fullname = "Administrator",
                Token = "12345",
                Userid = "1",
                Username = "administrator",
                Roles = new() { "Pengguna" },
                Password = "1qazXSW@"
            };

                    strUserFullNameNRole = user.Fullname + " (" + user.Roles + ")";
                    accessToken = user.Token ?? string.Empty;

                    var permissionResponse = await _PBTAuthUserService.LookupPermissionFromAPIAsync(accessToken);
                    if (permissionResponse.ReturnCode == 200)
                    {
                        string? permissionDataString = permissionResponse?.Data?.ToString();
                        if (!string.IsNullOrWhiteSpace(permissionDataString))
                        {
                            var test = JsonConvert.DeserializeObject<List<AuthenticatedMenuPermission>>(permissionDataString);
                            Permissions = JsonConvert.DeserializeObject<List<AuthenticatedMenuPermission>>(permissionDataString);
                        }
                    }

                    await _PBTAuthUserService.PersistUserToBrowserAsync(user);
                    principal = user.ToClaimsPrincipal();
                }
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
        catch (Exception ex)
        {
            result = new ReturnViewModel();
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await _PBTAuthUserService.ClearBrowserUserDataAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }

    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
}
