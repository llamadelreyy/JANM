//////using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
//////using Newtonsoft.Json;

//////namespace AllocationReport.Data;

//////public class AllocationUserService
//////{
//////    private readonly ProtectedLocalStorage _protectedLocalStorage;
//////    private readonly string _JainjStorageKey = "JainjIdentity";
//////    private readonly UserService _userService;
//////    public IConfiguration _configuration { get; }

//////    public AllocationUserService(ProtectedLocalStorage protectedLocalStorage, UserService userService, IConfiguration configuration)
//////    {
//////        _configuration = configuration;
//////        _protectedLocalStorage = protectedLocalStorage;
//////        _userService = userService;
//////    }

//////    public system_user? LookupUserInDatabaseAsync(string username, string password)
//////    {
//////        string? strUserName = _configuration["SuperUserName"] == null ? "superuser" : _configuration["SuperUserName"];
//////        string? strPassword = _configuration["SuperPassword"] == null ? "bismiLLah" : _configuration["SuperPassword"];

//////        //if login by super user
//////        var usersFromDatabase = new List<system_user>()
//////        {
//////            new()
//////            {
//////                userID = 9999,
//////                userName = strUserName,
//////                userPassword = strPassword,
//////                userFullName = "Super User",
//////                userEmail = "super_user@gmail.com",
//////                Roles=new()
//////                {
//////                    "superuser"
//////                },
//////                userRoles = "superuser"
//////            }
//////        };

//////        //Get all user form database
//////        if ((username != strUserName) && (password != strPassword))
//////            usersFromDatabase = _userService.GetLookupUserInDatabase();

//////        //////var usersFromDatabase = new List<system_user>()
//////        //////{
//////        //////    new()
//////        //////    {
//////        //////        userName = "blazorschool_normal",
//////        //////        userPassword = "blazorschool",
//////        //////        Roles =new()
//////        //////        {
//////        //////            "normal_user"
//////        //////        }
//////        //////    },
//////        //////    new()
//////        //////    {
//////        //////        userName = "user1",
//////        //////        userPassword = "1qazXSW@",
//////        //////        Roles =new()
//////        //////        {
//////        //////            "normal_user",
//////        //////            "paid_user"
//////        //////        }
//////        //////    },
//////        //////    new()
//////        //////    {
//////        //////        userName = "admin",
//////        //////        userPassword = "1qazXSW@",
//////        //////        Roles =new()
//////        //////        {
//////        //////            "normal_user",
//////        //////            "paid_user",
//////        //////            "admin"
//////        //////        }
//////        //////    },
//////        //////    //new()
//////        //////    //{
//////        //////    //    Username = "blazorschool_adult_user",
//////        //////    //    Password = "blazorschool",
//////        //////    //    Roles =new()
//////        //////    //    {
//////        //////    //        "normal_user"
//////        //////    //    },
//////        //////    //    Age = 22
//////        //////    //},
//////        //////    new()
//////        //////    {
//////        //////        userName = "blazorschool_adult_admin",
//////        //////        userPassword = "blazorschool",
//////        //////        Roles =new()
//////        //////        {
//////        //////            "normal_user",
//////        //////            "admin"
//////        //////        }
//////        //////    }
//////        //////};

//////        var foundUser = usersFromDatabase.SingleOrDefault(u => u.userName == username && u.userPassword == password);

//////        return foundUser;
//////    }

//////    public async Task PersistUserToBrowserAsync(system_user user)
//////    {
//////        string userJson = JsonConvert.SerializeObject(user);
//////        await _protectedLocalStorage.SetAsync(_JainjStorageKey, userJson);
//////    }

//////    public async Task<system_user?> FetchUserFromBrowserAsync()
//////    {
//////        try
//////        {
//////            var storedUserResult = await _protectedLocalStorage.GetAsync<string>(_JainjStorageKey);

//////            if (storedUserResult.Success && !string.IsNullOrEmpty(storedUserResult.Value))
//////            {
//////                var user = JsonConvert.DeserializeObject<system_user>(storedUserResult.Value);

//////                return user;
//////            }
//////        }
//////        catch (InvalidOperationException)
//////        {
//////        }

//////        return null;
//////    }

//////    public async Task ClearBrowserUserDataAsync() => await _protectedLocalStorage.DeleteAsync(_JainjStorageKey);
//////}