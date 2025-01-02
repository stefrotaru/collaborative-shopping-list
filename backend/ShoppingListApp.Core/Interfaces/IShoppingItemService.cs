public interface IShoppingItemService
{
    Task<ShoppingItemDto> AddShoppingItemAsync(string name, int quantity, int shoppingListId, int createdById);
    Task<ShoppingItemDto> GetShoppingItemByIdAsync(int shoppingItemId);
    Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsByShoppingListIdAsync(int shoppingListId);
    Task UpdateShoppingItemAsync(int shoppingItemId, string name, int quantity, bool isChecked);
    Task DeleteShoppingItemAsync(int shoppingItemId);
}
