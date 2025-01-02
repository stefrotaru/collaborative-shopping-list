public interface IShoppingListService
{
    Task<ShoppingListDto> CreateShoppingListAsync(string name, int groupId, int createdById);
    Task<ShoppingListDto> GetShoppingListByIdAsync(int shoppingListId);
    Task<IEnumerable<ShoppingListDto>> GetShoppingListsByGroupIdAsync(int groupId);
    Task UpdateShoppingListAsync(int shoppingListId, string name);
    Task DeleteShoppingListAsync(int shoppingListId);
}
