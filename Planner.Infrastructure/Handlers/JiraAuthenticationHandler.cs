using System.Text;
using Microsoft.Extensions.Options;
using Planner.Infrastructure.Options;

namespace Planner.Infrastructure.Handlers;

/// <summary>
/// A delegating handler that automatically attaches the Basic Authentication header 
/// to all outgoing HTTP requests directed to the Jira API.
/// </summary>
public class JiraAuthenticationHandler(IOptions<JiraApiOptions> options) : DelegatingHandler
{
    /// <summary>
    /// Intercepts the HTTP request to inject the Authorization header before sending it.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        
        // Generate the Base64 encoded credentials required by Jira Basic Auth
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.Email}:{settings.ApiToken}"));

        // Attach the authorization header to the current request
        request.Headers.Authorization = new("Basic", credentials);

        // Proceed with the execution of the HTTP request
        return await base.SendAsync(request, cancellationToken);
    }
}