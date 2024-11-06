using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerAPIPathPrefixInserter : IDocumentFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SwaggerAPIPathPrefixInserter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        string _pathPrefix = request?.Headers["X-Forwarded-Prefix"].FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(_pathPrefix))
        {
            return;
        }

        var paths = swaggerDoc.Paths.ToList();

        // Iterate over each existing path
        foreach (var path in paths)
        {
            // Clone the path entry
            var pathToChange = path.Value;

            // Remove the original path
            swaggerDoc.Paths.Remove(path.Key);

            // Add the new path with the prefix
            swaggerDoc.Paths.Add($"{_pathPrefix}{path.Key}", pathToChange);
        }
    }
}
