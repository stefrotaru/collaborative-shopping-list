public interface IShoppingListRepository
{
    Task<ShoppingList> GetByIdAsync(int id);
    Task<IEnumerable<ShoppingList>> GetByGroupIdAsync(int groupId);
    Task<IEnumerable<ShoppingList>> GetByGroupIdsAsync(int[] groupIds);
    Task AddAsync(ShoppingList shoppingList);
    Task UpdateAsync(ShoppingList shoppingList);
    Task DeleteAsync(ShoppingList shoppingList);
}
