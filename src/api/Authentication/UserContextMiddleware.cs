using Serilog;
using Microsoft.Identity.Web;
public class UserContextMiddleware
{
  private readonly RequestDelegate _next;

  public UserContextMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext, IUserContext userContext)
  {
    Log.Logger.Information("UserContextMiddelware invoked!");

    if (httpContext.User.Identity?.IsAuthenticated == true)
    {
      Log.Logger.Information("User is authenticated!");
      userContext.UserId = httpContext.User.GetObjectId();

      userContext.Email = httpContext.User.FindFirst("preferred_username")?.Value
                       ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    }

    await _next(httpContext);
  }
}