public interface IShoppingListAuthorizationService
{
    Task<bool> CanAccessShoppingListAsync(int userId, int shoppingListId);
    Task<List<AccessibleShoppingListDto>> GetAccessibleShoppingListsAsync(int userId);
}