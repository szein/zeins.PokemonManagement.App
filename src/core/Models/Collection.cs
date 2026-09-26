namespace core.Models;

public class Collection
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public DateTime CreateDate { get; set; } = DateTime.Now;

  public Guid UserId { get; set; }

  public ICollection<CollectionItem> CollectionItems { get; set; } = new List<CollectionItem>();
}