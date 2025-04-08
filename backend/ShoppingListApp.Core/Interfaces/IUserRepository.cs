public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByTokenAsync(string token);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);

    // User stats
    Task<int> GetUserShoppingListsCountAsync(int userId);
    Task<int> GetUserGroupsCountAsync(int userId);
    Task<int> GetUserItemsAddedCountAsync(int userId);
    Task<int> GetUserItemsCompletedCountAsync(int userId);
}
