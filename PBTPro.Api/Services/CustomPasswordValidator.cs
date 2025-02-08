using Microsoft.AspNetCore.Identity;
using PBTPro.DAL;

namespace PBTPro.Api.Services
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            var errors = new List<IdentityError>();

            if (password.Length != 8)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordLength",
                    Description = "Password must be exactly 8 characters long."
                });
            }

            var dayOfBirth = password.Substring(0, 2);
            if (!int.TryParse(dayOfBirth, out int day) || day < 1 || day > 31)
            {
                errors.Add(new IdentityError
                {
                    Code = "InvalidDayOfBirth",
                    Description = "The first 2 characters of the password must be a valid day of birth (01-31)."
                });
            }

            var stateCode = password.Substring(2, 2);
            if (!int.TryParse(stateCode, out int state) || state < 1 || state > 99)
            {
                errors.Add(new IdentityError
                {
                    Code = "InvalidStateCode",
                    Description = "The next 2 characters must be a valid state code (01-99)."
                });
            }

            var icLastDigits = password.Substring(4, 4);
            if (!int.TryParse(icLastDigits, out _))
            {
                errors.Add(new IdentityError
                {
                    Code = "InvalidICNumber",
                    Description = "The last 4 characters of the password must be numeric (IC number)."
                });
            }

            //if (!password.Any(char.IsUpper))
            //{
            //    errors.Add(new IdentityError
            //    {
            //        Code = "MissingUppercase",
            //        Description = "Password must contain at least one uppercase letter."
            //    });
            //}

            //if (!password.Any(char.IsLower))
            //{
            //    errors.Add(new IdentityError
            //    {
            //        Code = "MissingLowercase",
            //        Description = "Password must contain at least one lowercase letter."
            //    });
            //}

            //if (!password.Any(char.IsDigit))
            //{
            //    errors.Add(new IdentityError
            //    {
            //        Code = "MissingDigit",
            //        Description = "Password must contain at least one numeric digit."
            //    });
            //}

            //var specialCharacters = "#._@-!";
            //if (!password.Any(c => specialCharacters.Contains(c)))
            //{
            //    errors.Add(new IdentityError
            //    {
            //        Code = "MissingSpecialCharacter",
            //        Description = "Password must contain at least one special character from '#._@-!'."
            //    });
            //}

            return errors.Any() ? Task.FromResult(IdentityResult.Failed(errors.ToArray())) : Task.FromResult(IdentityResult.Success);
        }
    }
}
