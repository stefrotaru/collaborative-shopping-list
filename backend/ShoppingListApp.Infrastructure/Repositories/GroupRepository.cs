using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

public class GroupRepository : IGroupRepository
{
    private readonly ShoppingListContext _context;

    public GroupRepository(ShoppingListContext context)
    {
        _context = context;
    }

    public async Task<Group> GetByIdAsync(int id)
    {
        return await _context.Groups
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Group>> GetAllAsync()
    {
        return await _context.Groups
            .Include(g => g.GroupMembers)
            .ToListAsync();
    }

    public async Task<IEnumerable<Group>> GetByUserIdAsync(int userId)
    {
        return await _context.Groups
            .Include(g => g.GroupMembers)
            .Where(g => g.CreatedById == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Group group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Group group)
    {
        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Group group)
    {
        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
    }
    
    public async Task AddUserToGroupAsync(GroupMember groupMember)
    {
        _context.GroupMembers.Add(groupMember);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserFromGroupAsync(int groupId, int userId)
    {
        var groupMember = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

        if (groupMember != null)
        {
            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId)
    {
        return await _context.GroupMembers
            .Include(gm => gm.User)
            .Where(gm => gm.GroupId == groupId)
            .ToListAsync();
    }
}