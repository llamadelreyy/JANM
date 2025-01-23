using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PBTPro.DAL.Models.CommonServices;
using System.Text;

namespace PBTPro.DAL.Services;

public class PBTAuthUserService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly string _AuthStorageKey = "AuthIdentity";
    private readonly ApiConnector _ApiConnector;
    public IConfiguration _configuration { get; }

    public PBTAuthUserService(ProtectedLocalStorage protectedLocalStorage, ApiConnector ApiConnector, IConfiguration configuration)
    {
        _configuration = configuration;
        _protectedLocalStorage = protectedLocalStorage;
        _ApiConnector = ApiConnector;
    }

    public async Task<ReturnViewModel> LookupUserFromAPIAsync(string username, string password)
    {
        var result = new ReturnViewModel();
        try
        {
            LoginModel loginModel = new LoginModel { Password = password, Username = username, RememberMe = true };
            var reqData = JsonConvert.SerializeObject(loginModel);
            var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

            string requestUrl = $"/api/Authenticate/login";
            var response = await _ApiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

            result = response;
        }
        catch (Exception ex)
        {
            result = new ReturnViewModel();
            result.ReturnMessage = ex.Message;
            result.ReturnCode = 500;
        }

        return result;
    }

    public async Task<ReturnViewModel> LookupPermissionFromAPIAsync(string accessToken)
    {
        var result = new ReturnViewModel();
        try
        {
            string requestUrl = $"/api/User/GetUserMenuPermission";
            _ApiConnector.accessToken = accessToken;
            var response = await _ApiConnector.ProcessLocalApi(requestUrl);

            result = response;
        }
        catch (Exception ex)
        {
            result = new ReturnViewModel();
            result.ReturnMessage = ex.Message;
            result.ReturnCode = 500;
        }

        return result;
    }

    public async Task<ReturnViewModel> ForgotPasswordAsync(ForgetPasswordInput InputModel)
    {
        var result = new ReturnViewModel();
        try
        {
            var reqData = JsonConvert.SerializeObject(InputModel);
            var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

            string requestUrl = $"/api/Authenticate/ForgotPasswordModel";
            var response = await _ApiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

            result = response;
        }
        catch (Exception ex)
        {
            result = new ReturnViewModel();
            result.ReturnMessage = ex.Message;
            result.ReturnCode = 500;
        }

        return result;
    }


    public async Task<ReturnViewModel> ResetPasswordAsync(ResetPasswordInput InputModel)
    {
        var result = new ReturnViewModel();
        try
        {
            var reqData = JsonConvert.SerializeObject(InputModel);
            var reqContent = new StringContent(reqData, Encoding.UTF8, "application/json");

            string requestUrl = $"/api/Authenticate/ResetPassword";
            var response = await _ApiConnector.ProcessLocalApi(requestUrl, HttpMethod.Post, reqContent);

            result = response;
        }
        catch (Exception ex)
        {
            result = new ReturnViewModel();
            result.ReturnMessage = ex.Message;
            result.ReturnCode = 500;
        }

        return result;
    }

    public async Task PersistUserToBrowserAsync(AuthenticatedUser user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        await _protectedLocalStorage.SetAsync(_AuthStorageKey, userJson);
    }

    public async Task<AuthenticatedUser?> FetchUserFromBrowserAsync()
    {
        try
        {
            var storedUserResult = await _protectedLocalStorage.GetAsync<string>(_AuthStorageKey);

            if (storedUserResult.Success && !string.IsNullOrEmpty(storedUserResult.Value))
            {
                var user = JsonConvert.DeserializeObject<AuthenticatedUser>(storedUserResult.Value);

                return user;
            }
        }
        catch (InvalidOperationException)
        {
        }

        return null;
    }

    public async Task ClearBrowserUserDataAsync() => await _protectedLocalStorage.DeleteAsync(_AuthStorageKey);

}