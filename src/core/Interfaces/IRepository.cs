namespace core.Inerfaces;

public interface IRepository<TItem> where TItem : class
{
    /// <summary>
    /// Get All Items
    /// </summary>
    /// <returns></returns>
    Task<List<TItem>> GetAllAsync();
    /// <summary>
    /// Get board occurrences in an Item.
    /// </summary>
    /// <param name="ItemId"></param>
    /// <returns></returns>
    Task<TItem?> GetByIdAsync(Guid id);
    /// <summary>
    /// Add Item to the database
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    Task<TItem> AddAsync(TItem item);
    /// <summary>
    /// Update an Item 
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    Task<TItem> UpdateAsync(TItem Item);
    /// <summary>
    /// Delete an Item by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}
