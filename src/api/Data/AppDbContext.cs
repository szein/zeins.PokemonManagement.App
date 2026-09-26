using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
  private readonly IUserContext _userContext;
  public DbSet<Collection> Collections { get; set; }
  public DbSet<CollectionItem> CollectionItems { get; set; }

  public AppDbContext(
      DbContextOptions<AppDbContext> options,
      IUserContext userContext) : base(options)
  {
    _userContext = userContext;
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Collection>()
        .HasMany(c => c.CollectionItems);
  }
}