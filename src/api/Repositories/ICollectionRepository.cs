
using core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


public interface ICollectionRepository
{
  Task<List<Collection>> GetAllAsync(Guid ownerId);
  Task<Collection?> GetByIdAsync(Guid id);
  Task<Collection> AddAsync(Collection component);
  Task<Collection> UpdateAsync(Collection component);
  Task<bool> DeleteAsync(Guid id);

  Task<CollectionItem> AddCollectionItemAsync(Guid collectionId, CollectionItem item);
  Task<bool> RemoveCollectionItemAsync(Guid collectionId, Guid collectionItemId);

}

