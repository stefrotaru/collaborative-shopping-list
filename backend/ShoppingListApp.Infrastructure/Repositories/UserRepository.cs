using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ShoppingListContext _context;

    public UserRepository(ShoppingListContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    } 
    public async Task<User> GetByTokenAsync(string token)
    {
        return  await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    // User stats
    public async Task<int> GetUserShoppingListsCountAsync(int userId)
    {
        return await _context.ShoppingLists
            .CountAsync(list => list.CreatedById == userId);
    }

    public async Task<int> GetUserGroupsCountAsync(int userId)
    {
        // Count groups created by user
        var createdGroups = await _context.Groups
            .CountAsync(group => group.CreatedById == userId);

        // Count groups where user is a member but not the admin
        var memberGroups = await _context.GroupMembers
            .CountAsync(member => member.UserId == userId && member.Role != "Admin");

        return createdGroups + memberGroups;
    }

    public async Task<int> GetUserItemsAddedCountAsync(int userId)
    {
        return await _context.ShoppingItems
            .CountAsync(item => item.CreatedById == userId);
    }

    public async Task<int> GetUserItemsCompletedCountAsync(int userId)
    {
        return await _context.ShoppingItems
            .CountAsync(item => item.CreatedById == userId && item.IsChecked);
    }
}
