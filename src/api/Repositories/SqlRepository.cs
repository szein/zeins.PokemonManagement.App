using core.Inerfaces;
using Microsoft.EntityFrameworkCore;

public class SqlRepository<T> : IRepository<T>
    where T : class
{
  private readonly AppDbContext _context;
  private readonly DbSet<T> _dbSet;

  public SqlRepository(AppDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  public async Task<List<T>> GetAllAsync()
  {
    return await _dbSet.ToListAsync();
  }

  public async Task<T?> GetByIdAsync(Guid id)
  {
    return await _dbSet.FindAsync(id);
  }

  public async Task<T> AddAsync(T item)
  {
    await _dbSet.AddAsync(item);
    await _context.SaveChangesAsync();
    return item;
  }

  public async Task<T> UpdateAsync(T item)
  {
    _dbSet.Update(item);
    await _context.SaveChangesAsync();
    return item;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var entity = await _dbSet.FindAsync(id);

    if (entity == null)
      return false;

    _dbSet.Remove(entity);
    await _context.SaveChangesAsync();

    return true;
  }
}