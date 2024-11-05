using Microsoft.AspNetCore.Http.Extensions;
using System;

public class MiddlewareRemoveSseClientId
{
    private readonly RequestDelegate _next;

    public MiddlewareRemoveSseClientId(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Check if the request path matches the SignalR hub
        if (context.Request.Path.StartsWithSegments("/pushDataHub"))
        {
            var originalUrl = context.Request.GetDisplayUrl();
            int isSseClientId = originalUrl.IndexOf("?sseClientId");

            if (isSseClientId > 0)
            {
                string[] queryParts = originalUrl.Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

                if (queryParts.Length > 1 && !string.IsNullOrWhiteSpace(queryParts[1]))
                {
                    context.Request.QueryString = new QueryString("?" + queryParts[1]);
                }
                else
                {
                    context.Request.QueryString = QueryString.Empty;
                }
            }
        }
        await _next(context);
    }
}