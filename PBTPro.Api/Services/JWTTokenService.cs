using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JWTTokenService
{
    private readonly IConfiguration _config;

    public JWTTokenService(IConfiguration config)
    {
        _config = config;
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
}