using DevExpress.Blazor.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PBTPro.DAL;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JWTTokenService
{
    private readonly IConfiguration _config;
    private readonly PBTProDbContext _dbContext;

    public JWTTokenService(IConfiguration config,
        PBTProDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    public string GenerateJwtToken(List<Claim> authClaims, bool rememberMe = false)
    {
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
        var creds = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

        authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        
        var expiration = rememberMe ? DateTime.Now.AddYears(1) : DateTime.Now.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: _config["JWT:ValidIssuer"],
            audience: _config["JWT:ValidAudience"],
            claims: authClaims,
            expires: expiration,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            return false;
        }

        var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return false;
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return false; 
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == userId);
        if (user == null)
        {
            return false; 
        }

        if (user.IsDeleted) 
        {
            return false;
        }

        var roleDB = await _dbContext.UserRoles.Where(x => x.UserId == userId && x.IsDeleted == false).Select(x=>x.RoleId).ToListAsync();
        var roleClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "RoleIds")?.Value;
        var roleList = new List<int>();
        //roleList = roleClaim.Select(role =>
        //{
        //    int roleId;
        //    return int.TryParse(role, out roleId) ? roleId : (int?)null;
        //})
        //.Where(roleId => roleId.HasValue)
        //.Select(roleId => roleId.Value)
        //.ToList();

        roleList = roleClaim?.Split(',')
                        .Select(role =>
                        {
                            int roleId;
                            return int.TryParse(role, out roleId) ? roleId : (int?)null;
                        })
                        .Where(roleId => roleId.HasValue)
                        .Select(roleId => roleId.Value)
                        .ToList() ?? new List<int>();

        if (roleDB.Count != roleList.Count)
        {
            return false;
        }
        else
        {
            var rolesInTokenNotInDB = roleList.Except(roleDB).ToList();
            var rolesInDBNotInToken = roleDB.Except(roleList).ToList();

            if (rolesInTokenNotInDB.Any() || rolesInDBNotInToken.Any())
            {
                return false;
            }
        }

        return true;
    }
}