using api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom.Compiler;
using static api.DTOs.PokemonDTOs;

[ApiController]
[Route("api/[controller]")]
public class PokemonsController : ControllerBase
{
  private readonly IPokemonService _pokeApiService;
  private readonly ILogger<PokemonsController> _logger;

  public PokemonsController(IPokemonService pokeApiService, ILogger<PokemonsController> logger)
  {
    _pokeApiService = pokeApiService;
    _logger = logger;
  }

  //TODO: use in memory cach
  // GET api/pokemons?limit=20&offset=0
  [HttpGet]
  public async Task<ActionResult<PokemonStoreResponse>> GetPokemons(
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      CancellationToken ct = default)
  {
    _logger?.LogInformation("GetPokemons called (page={Page}, pageSize={PageSize})", page, pageSize);
    var limit = pageSize;
    var offset = (page - 1) * pageSize;
    var pokemonsResult = await _pokeApiService.GetPokemonsAsync(limit, offset, ct);

    var getPokemonsTasks = pokemonsResult.Results.Select(x =>
          _pokeApiService.GetPokemonAsync(x.Name, ct));

    var details = await Task.WhenAll(getPokemonsTasks);
    var items = details
        .Where(x => x is not null)
        .Select(x => new PokemonDTO(
          x!.Id,
          x.Name,//TODO: char.ToUpper(x.Name[0]) + x.Name.Substring(1);
          x.Height,
          x.Weight,
          "https://pokeapi.co/api/v2/pokemon/"+x.Id
          )
        ).ToList();
    return Ok(new PokemonStoreResponse(items, pokemonsResult.Count));
  }

  // GET api/pokemons/pikachu  (or /api/pokemons/25)
  [HttpGet("{nameOrId}")]
  public async Task<ActionResult<PokemonDTO>> GetPokemon(string nameOrId, CancellationToken ct = default)
  {
    _logger?.LogInformation("GetPokemon called for {NameOrId}", nameOrId);
    var pokemon = await _pokeApiService.GetPokemonAsync(nameOrId, ct);
    if (pokemon is null)
    {
      _logger?.LogWarning("Pokemon not found: {NameOrId}", nameOrId);
      return NotFound();
    }
    return Ok(pokemon);
  }
}