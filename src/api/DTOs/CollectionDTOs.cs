using static api.DTOs.PokemonDTOs;

namespace api.DTOs
{
  public class CollectionDTOs
  {
    public record CollectionResponse(string Id, string OwnerId, string Name,  List<PokemonDTO> Pokemons);
    public record CreateCollectionRequest(string? Name);
    public record AddPokemonRequest(string? Name, string? Url);
    
  }
}
