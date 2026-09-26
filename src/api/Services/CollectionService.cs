using static api.DTOs.CollectionDTOs;
using static api.DTOs.PokemonDTOs;

namespace api.Services
{
  public class CollectionService : ICollectionService
  {
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private readonly IUserContext _userContext;
    private readonly AppDbContext _context;
    private readonly ILogger<CollectionService> _logger;
    public CollectionService(AppDbContext context, IUserContext userContext, ILogger<CollectionService> logger)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

    }

    public async Task<CollectionResponse> CreateAsync(string name)
    {
      _logger?.LogInformation("CreateAsync called for owner {OwnerId} name={Name}", _userContext.UserId, name);
      await _lock.WaitAsync();
      try
      {
        _context.Collections.Add(new Collection { Name = name, UserId = _userContext.GetUserId() });
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error creating collection for owner {OwnerId}", _userContext.UserId);
      }
      finally
      {
        _lock.Release();
      }
      return new CollectionResponse("", _userContext.UserId ?? "", name, new List<PokemonDTO>());
    }

    public Task<bool> DeleteAsync(Guid id)
    {
      throw new NotImplementedException();
    }

    public async Task<List<CollectionResponse>> GetAllAsync()
    {
      _logger?.LogInformation("GetAllAsync called for user {UserId}", _userContext.UserId);
      try
      {
        if (!_context.Collections.Any(x => x.UserId == _userContext.GetUserId()))
        {
          await CreateAsync("My Default Collection");
        }

        var collections = _context.Collections.Where(c => c.UserId == _userContext.GetUserId())
          .Select(x => new CollectionResponse(
            x.Id.ToString(),
            x.UserId.ToString(),
            x.Name,
            x.CollectionItems.Select(ci => new PokemonDTO(int.Parse(ci.PokemonId), ci.PokemonName, int.Parse(ci.PokemonHeight), int.Parse(ci.PokemonWeight), ci.PokemonUrl, ci.Id)).ToList()
          ))
        .ToList();
        return collections;

      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error fetching collections for user {UserId}", _userContext.UserId);
      }
      return null;
    }

    public async Task<Collection?> GetByIdAsync(Guid id)
    {
      _logger?.LogInformation("GetByIdAsync called for id {Id}", id);
      try
      {
        var collection = await _context.Collections.FindAsync(id);
        if (collection == null) return null;

        return collection;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error fetching collection {Id}", id);
        return null;
      }
    }

    public async Task<ServiceResult<Collection?>> TryGetUserCollection(Guid collectionId)
    {
      _logger?.LogInformation("TryGetUserCollection called for collection {CollectionId} user {UserId}", collectionId, _userContext.UserId);
      try
      {
        var collection = await _context.Collections.FindAsync(collectionId);
        if (collection == null)
        {
          _logger?.LogWarning("Collection not found: {CollectionId}", collectionId);
          return ServiceResult<Collection?>.NotFound("Collection not found");
        }

        if (collection.UserId != _userContext.GetUserId())
        {
          _logger?.LogWarning("Forbidden access to collection {CollectionId} by user {UserId}", collectionId, _userContext.UserId);
          return ServiceResult<Collection?>.Forbidden("You do not have access to this collection");
        }

        return ServiceResult<Collection?>.Ok(collection);
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error fetching collection {CollectionId} for user {UserId}", collectionId, _userContext.UserId);
        return ServiceResult<Collection?>.Error("An error occurred while fetching the collection");
      }
    }

    public async Task<CollectionItem> AddItem(Guid collectionId, CollectionItem item)
    {
      _logger?.LogInformation("AddItem called for collection {CollectionId}", collectionId);
      try
      {
        var collection = _context.Collections.SingleOrDefault(c => c.Id == collectionId);
        _logger?.LogDebug("Collection with Id {CollectionId} was {Found}", collectionId, collection == null ? "NotFound" : "Found");
        collection?.CollectionItems.Add(item);
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error adding item to collection {CollectionId}", collectionId);
      }
      return item;
    }

    public async Task<bool> RemoveItem(Guid collectionId, Guid itemId)
    {
      _logger?.LogInformation("RemoveItem called for collection {CollectionId} item {ItemId}", collectionId, itemId);
      try
      {
        
        var item = _context.CollectionItems.FirstOrDefault(ci => ci.Id == itemId);
        _logger?.LogDebug("CollectionItem with Id {ItemId} was {Found}", itemId, item == null ? "NotFound" : "Found");
        if (item == null) return false;

        _context?.CollectionItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error removing item {ItemId} from collection {CollectionId}", itemId, collectionId);
        return false;
      }
    }

    public async Task<Collection?> UpdateAsync(Guid id, string name)
    {
      _logger?.LogInformation("UpdateAsync called for id {Id}", id);
      try
      {
        var collection = _context.Collections.SingleOrDefault(c => c.Id == id);
        _logger?.LogDebug("Collection with Id {Id} was {Found}", id, collection == null ? "NotFound" : "Found");
        if (collection == null) return null;

        collection.Name = name;
        await _context.SaveChangesAsync();
        return collection;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error updating collection {Id}", id);
        return null;
      }
    }
  }
}
