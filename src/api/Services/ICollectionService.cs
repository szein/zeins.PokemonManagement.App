using core.Models;
using static api.DTOs.CollectionDTOs;

public interface ICollectionService
{
  /// <summary>
  /// Get all Collections
  /// </summary>
  /// <returns>List of Collections</returns>
  Task<List<CollectionResponse>> GetAllAsync();
  /// <summary>
  /// Get Collection by its Id
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<Collection?> GetByIdAsync(Guid id);
  /// <summary>
  /// Create new Collection
  /// </summary>
  /// <param name="name">Collection name</param>
  /// 
  /// <returns></returns>
  Task<CollectionResponse> CreateAsync(string name);

  /// <summary>
  /// Add new item to collection
  /// </summary>
  /// /// <param name="item">Collection id</param>
  /// <param name="item">CollectionItem to add</param>
  /// <returns></returns>
  Task<CollectionItem> AddItem(Guid collectionId, CollectionItem item);

  /// <summary>
  /// Remove item from collection
  /// </summary>
  /// <param name="name">item id</param>
  /// <returns></returns>
  Task<bool> RemoveItem(Guid collectionId, Guid itemId);


  /// <summary>
  /// Update the Collection data
  /// </summary>
  /// <param name="id"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  Task<Collection?> UpdateAsync(Guid id, string name);
  /// <summary>
  /// Delete Collection by its id
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<bool> DeleteAsync(Guid id);

  /// <summary>
  /// Get a collection and validate it belongs to the specified user. Returns a service result
  /// that carries user-facing error messages for controller mapping.
  /// </summary>
  Task<ServiceResult<Collection?>> TryGetUserCollection(Guid collectionId);
}
