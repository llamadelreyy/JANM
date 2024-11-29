//////using Microsoft.AspNetCore.Components.Authorization;
//////using System.Security.Claims;

//////namespace AllocationReport.Data;

//////public class AllocationAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
//////{
//////    private readonly AllocationUserService _allocationUserService;
//////    public system_user CurrentUser { get; private set; } = new();
//////    public string strUserFullNameNRole { get; set; } = "";

//////    public AllocationAuthenticationStateProvider(AllocationUserService allocationUserService)
//////    {
//////        _allocationUserService = allocationUserService;
//////        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
//////    }

//////    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
//////    {
//////        var authenticationState = await task;

//////        if (authenticationState is not null)
//////        {
//////            CurrentUser = system_user.FromClaimsPrincipal(authenticationState.User);
//////            if (CurrentUser.userID == 0)
//////                await GetAuthenticationStateAsync();
//////        }
//////    }

//////    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//////    {
//////        var principal = new ClaimsPrincipal();
//////        var user = await _allocationUserService.FetchUserFromBrowserAsync();

//////        if (user is not null)
//////        {
//////            var userInDatabase = _allocationUserService.LookupUserInDatabaseAsync(user.userName, user.userPassword);

//////            if (userInDatabase is not null)
//////            {
//////                principal = userInDatabase.ToClaimsPrincipal();
//////                CurrentUser = userInDatabase;
//////                strUserFullNameNRole = user.userFullName + " (" + user.userRoles + ")";
//////            }
//////        }

//////        return new(principal);
//////    }

//////    public async Task LoginAsync(string username, string password)
//////    {
//////        var principal = new ClaimsPrincipal();
//////        var user = _allocationUserService.LookupUserInDatabaseAsync(username, password);

//////        if (user is not null)
//////        {
//////            strUserFullNameNRole = user.userFullName + " (" + user.userRoles + ")";
//////            await _allocationUserService.PersistUserToBrowserAsync(user);
//////            principal = user.ToClaimsPrincipal();
//////        }

//////        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
//////    }

//////    public async Task LogoutAsync()
//////    {
//////        await _allocationUserService.ClearBrowserUserDataAsync();
//////        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
//////    }

//////    public void Dispose() => AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
//////}
