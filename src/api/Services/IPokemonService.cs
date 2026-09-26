using static api.DTOs.PokemonDTOs;

namespace api.Services
{
  public interface IPokemonService
  {
    Task<PokemonListResponse> GetPokemonsAsync(int limit = 20, int offset = 0, CancellationToken ct = default);
    Task<PokemonDTO?> GetPokemonAsync(string nameOrId, CancellationToken ct = default);

  }
}
