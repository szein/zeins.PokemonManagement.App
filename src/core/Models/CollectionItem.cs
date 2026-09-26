namespace core.Models;

public class CollectionItem
{
  public Guid Id { get; set; }
  public Guid CollectionId { get; set; }
  public string? PokemonId { get; set; }
  public string? PokemonUrl { get; set; }
  public string? PokemonName { get; set; }
  public string? PokemonHeight { get; set; }
  public string? PokemonWeight { get; set; }
  public DateTime AddedAt { get; set; } = DateTime.Now;

}