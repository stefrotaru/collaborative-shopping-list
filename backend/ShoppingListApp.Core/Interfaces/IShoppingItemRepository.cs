public interface IShoppingItemRepository
{
    Task<ShoppingItem> GetByIdAsync(int id);
    Task<IEnumerable<ShoppingItem>> GetAllAsync();
    Task<IEnumerable<ShoppingItem>> GetByShoppingListIdAsync(int shoppingListId);
    Task AddAsync(ShoppingItem shoppingItem);
    Task UpdateAsync(ShoppingItem shoppingItem);
    Task DeleteAsync(ShoppingItem shoppingItem);
}
