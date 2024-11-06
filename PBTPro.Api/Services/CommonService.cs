using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Runtime.CompilerServices;

namespace PBTPro.Api.Services
{
    public class CommonService : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ISession _session => _contextAccessor.HttpContext.Session;
        protected readonly string? _apiBaseUrl;
        public CommonService()
        {

        }
        public void LogConsole(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            // final output
            Console.WriteLine("(" + caller + ") " + message + " PASS at line " + lineNumber);
        }
        public void LogMessage(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            Log.Warning("(" + caller + ") " + message + " PASS at line " + lineNumber);
        }
        public void LogError(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string? caller = null)
        {
            Log.Error("(" + caller + ") " + message + " at line " + lineNumber);
        }
        public string CheckToken()
        {

            var accessToken = _session.GetString("Token");
            return accessToken;
        }

        public HttpRequestMessage CheckRequest(string RequestURL)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}" + RequestURL);
            return request;
        }
        public string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
            };

            Random rand = new Random(System.Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (opts.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (opts.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (opts.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
