#nullable disable
using api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static api.DTOs.CollectionDTOs;
using static api.DTOs.PokemonDTOs;

namespace api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class CollectionController : ControllerBase
  {
    private readonly IUserContext _userContext;
    private readonly ICollectionService _collectionService;
    private readonly ILogger<CollectionController> _logger;

    public CollectionController(IUserContext userContext, ICollectionService collectionService, ILogger<CollectionController> logger)
    {
      _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
      _collectionService = collectionService ?? throw new ArgumentNullException(nameof(collectionService));
      _logger = logger;
    }

    // GET: api/collection
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollectionResponse>>> GetAll()
    {
      var result = await _collectionService.GetAllAsync();

      return Ok(result);
    }

    // GET: api/collection/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Collection>> GetById(Guid id)
    {
      _logger?.LogInformation("GetById called for id {Id} user {UserId}", id, _userContext?.UserId);

      var collection = await _collectionService.TryGetUserCollection(id);
      return Ok(collection);
    }

    // POST: api/collection
    [HttpPost]
    public async Task<ActionResult<Collection>> Create([FromBody] CreateCollectionRequest request)
    {
      _logger?.LogInformation("Create collection called by user {UserId}", _userContext?.UserId);

      var created = await _collectionService.CreateAsync(request?.Name ?? string.Empty);
      return Ok(created);
    }

    // DELETE: api/collection/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
      _logger?.LogInformation("Delete collection {Id} called by user {UserId}", id, _userContext?.UserId);

      var collection = await _collectionService.TryGetUserCollection(id);

      var deleted = await _collectionService.DeleteAsync(id);
      if (!deleted)
      {
        _logger?.LogWarning("Delete failed - collection not found: {Id}", id);
        return NotFound();
      }

      return NoContent();
    }

    // POST: api/collection/{collectionId}/items
    [HttpPost("{collectionId:guid}/items")]
    public async Task<ActionResult<CollectionItem>> AddItem(Guid collectionId, [FromBody] PokemonDTO request)
    {
      _logger?.LogInformation("AddItem called on collection {CollectionId} by user {UserId}", collectionId, _userContext?.UserId);

      var collection = await _collectionService.TryGetUserCollection(collectionId);

      var item = await _collectionService.AddItem(collectionId,
        new CollectionItem()
        {
          CollectionId = collectionId,
          PokemonId = request.Id.ToString(),
          PokemonName = request.Name,
          PokemonUrl = request.Url,
          PokemonHeight = request.Height.ToString(),
          PokemonWeight = request.Weight.ToString()
        });
      return CreatedAtAction(nameof(GetById), new { id = collectionId }, item);
    }

    // DELETE: api/collection/{collectionId}/items/{itemId}
    [HttpDelete("{collectionId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid collectionId, Guid itemId)
    {
      _logger?.LogInformation("RemoveItem called on collection {CollectionId} item {ItemId} by user {UserId}", collectionId, itemId, _userContext?.UserId);

      var collection = await _collectionService.TryGetUserCollection(collectionId);

      var removed = await _collectionService.RemoveItem(collectionId, itemId);
      if (!removed)
        return NotFound();

      return NoContent();
    }
  }
}
