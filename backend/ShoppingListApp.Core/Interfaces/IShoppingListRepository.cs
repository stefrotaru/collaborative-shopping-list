public interface IShoppingListRepository
{
    Task<ShoppingList> GetByIdAsync(int id);
    Task<IEnumerable<ShoppingList>> GetAllAsync();
    Task<IEnumerable<ShoppingList>> GetByGroupIdAsync(int groupId);
    Task AddAsync(ShoppingList shoppingList);
    Task UpdateAsync(ShoppingList shoppingList);
    Task DeleteAsync(ShoppingList shoppingList);
}
