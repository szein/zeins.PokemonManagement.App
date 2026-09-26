using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace api.DTOs
{
  public class PokemonDTOs
  {
    public record PokemonListResponse(int Count, string? Next, string? Previous, List<PokemonListItem> Results);
    public record PokemonListItem(string Name, string Url);
    public record PokemonStoreResponse(List<PokemonDTO> Items, int TotalItems);
    public record PokemonDTO(int Id, string Name, int Height, int Weight, string? Url, Guid? itemId = null);

  }
}
