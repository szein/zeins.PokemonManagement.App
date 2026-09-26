using Microsoft.Identity.Client;

public interface IUserContext
{
  string? UserId { get; set; }
  string? Email { get; set; }
  string? TenantId { get; set; }

  Guid GetUserId();
}

public class UserContext : IUserContext
{
  public string? UserId { get; set; }
  public string? Email { get; set; }
  public string? TenantId { get; set; }

  public Guid GetUserId()
  {
    var userId = Guid.TryParse(UserId, out var id);
    return id;
  }

}