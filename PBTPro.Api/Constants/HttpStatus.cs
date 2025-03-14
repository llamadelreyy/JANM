namespace PBTPro.Api.Constants
{
    public enum HttpStatus
    {
        Success = 200,
        NoRequestAuthority = 401,
        NotFound = 404,
        NoContent = 204,
        Error = 500,
        BadRequest = 400,
        AuthenticationTimeout = 419
    }
}
