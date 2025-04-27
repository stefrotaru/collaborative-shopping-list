public interface IShoppingListRepository
{
    Task<ShoppingList> GetByIdAsync(int id);
    Task<IEnumerable<ShoppingList>> GetByGroupIdAsync(int groupId);
    Task<IEnumerable<ShoppingList>> GetByGroupIdsAsync(int[] groupIds);
    Task<List<ShoppingList>> GetByCreatedByIdAsync(int createdById);
    Task AddAsync(ShoppingList shoppingList);
    Task UpdateAsync(ShoppingList shoppingList);
    Task DeleteAsync(ShoppingList shoppingList);

    Task<ShoppingList> GetByGuidAsync(Guid guid);
    Task<bool> ExistsByGuidAsync(Guid guid);
    Task<IEnumerable<ShoppingList>> GetByGroupGuidAsync(Guid groupGuid);
}
