public interface IShoppingListService
{
    Task<ShoppingListDto> CreateShoppingListAsync(string name, int groupId, int createdById);
    Task<ShoppingListDto> GetShoppingListByIdAsync(int shoppingListId);
    Task<IEnumerable<ShoppingListDto>> GetShoppingListsByGroupIdAsync(int groupId);
    Task<IEnumerable<ShoppingListDto>> GetShoppingListsByGroupIdsAsync(int[] groupIds);
    Task<List<ShoppingListDto>> GetShoppingListsByCreatedByIdAsync(int createdById);
    Task UpdateShoppingListAsync(int shoppingListId, string name);
    Task DeleteShoppingListAsync(int shoppingListId);

    Task<ShoppingListDto> GetShoppingListByGuidAsync(Guid guid);
    Task<IEnumerable<ShoppingListDto>> GetShoppingListsByGroupGuidAsync(Guid groupGuid);


}
