using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerAPIPathPrefixInserter : IDocumentFilter
{
    private readonly string _pathPrefix;

    public SwaggerAPIPathPrefixInserter(string prefix)
    {
        _pathPrefix = prefix;
    }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Get the existing paths
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
