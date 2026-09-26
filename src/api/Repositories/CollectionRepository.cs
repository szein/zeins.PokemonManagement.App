using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Models;
using Microsoft.EntityFrameworkCore;

public class CollectionRepository : ICollectionRepository
{
  private readonly AppDbContext _context;

  public CollectionRepository(AppDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<List<Collection>> GetAllAsync(Guid ownerId)
  {
    return await _context.Collections
      .AsNoTracking()
      .Include(c => c.CollectionItems)
      .Where(c => c.UserId == ownerId)
      .ToListAsync();
  }

  public async Task<Collection?> GetByIdAsync(Guid id)
  {
    return await _context.Collections
      .Include(c => c.CollectionItems)
      .FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task<Collection> AddAsync(Collection component)
  {
    if (component.Id == Guid.Empty)
      component.Id = Guid.NewGuid();

    await _context.Collections.AddAsync(component);
    await _context.SaveChangesAsync();
    return component;
  }

  public async Task<Collection> UpdateAsync(Collection component)
  {
    _context.Collections.Update(component);
    await _context.SaveChangesAsync();
    return component;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var entity = await _context.Collections.FindAsync(id);
    if (entity == null)
      return false;

    _context.Collections.Remove(entity);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<CollectionItem> AddCollectionItemAsync(Guid collectionId, CollectionItem item)
  {
    if (item.Id == Guid.Empty)
      item.Id = Guid.NewGuid();

    item.CollectionId = collectionId;

    // ensure collection exists
    var collection = await _context.Collections
      .Include(c => c.CollectionItems)
      .FirstOrDefaultAsync(c => c.Id == collectionId);

    if (collection != null)
    {
      collection.CollectionItems.Add(item);
    }
    else
    {
      // still add item so it gets tracked, but this should normally not happen
      await _context.Set<CollectionItem>().AddAsync(item);
    }

    await _context.SaveChangesAsync();
    return item;
  }

  public async Task<bool> RemoveCollectionItemAsync(Guid collectionId, Guid collectionItemId)
  {
    var item = await _context.Set<CollectionItem>()
      .FirstOrDefaultAsync(i => i.Id == collectionItemId && i.CollectionId == collectionId);

    if (item == null)
      return false;

    _context.Set<CollectionItem>().Remove(item);
    await _context.SaveChangesAsync();
    return true;
  }
}
