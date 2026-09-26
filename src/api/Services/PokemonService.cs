using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static api.DTOs.PokemonDTOs;

namespace api.Services
{
  public class PokemonService : IPokemonService
  {
    private readonly HttpClient _http;
    private readonly ILogger<PokemonService> _logger;
    private readonly IMemoryCache _cache;

    public PokemonService(HttpClient http, ILogger<PokemonService> logger, IMemoryCache cache)
    {
      _http = http;
      _logger = logger;
      _cache = cache;
    }

    public async Task<PokemonListResponse> GetPokemonsAsync(int limit = 20, int offset = 0, CancellationToken ct = default)
    {
      _logger?.LogInformation("GetPokemonsAsync called (limit={Limit}, offset={Offset})", limit, offset);
      var cacheKey = $"pokemon_list_{limit}_{offset}";

      var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        var res = await _http.GetFromJsonAsync<PokemonListResponse>($"pokemon?limit={limit}&offset={offset}", ct);
        return res ?? new PokemonListResponse(0, null, null, new());
      });

      return result;
    }

    public async Task<PokemonDTO?> GetPokemonAsync(string nameOrId, CancellationToken ct = default)
    {
      _logger?.LogInformation("GetPokemonAsync called for {NameOrId}", nameOrId);
      try
      {
        var key = $"pokemon_{nameOrId.ToLower()}";

        if (_cache.TryGetValue<PokemonDTO?>(key, out var cached))
        {
          return cached;
        }

        var fetched = await _http.GetFromJsonAsync<PokemonDTO>($"pokemon/{nameOrId.ToLower()}", ct);

        if (fetched is not null)
        {
          _cache.Set(key, fetched, TimeSpan.FromMinutes(5));
        }

        return fetched;
      }
      catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
      {
        _logger?.LogWarning("Pokemon not found: {NameOrId}", nameOrId);
        return null;
      }
    }
  }
}
