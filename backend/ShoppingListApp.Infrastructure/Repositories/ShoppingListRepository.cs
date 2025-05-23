﻿using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

public class ShoppingListRepository : IShoppingListRepository
{
    private readonly ShoppingListContext _context;
    public ShoppingListRepository(ShoppingListContext context)
    {
        _context = context;
    }
    public async Task<ShoppingList> GetByIdAsync(int id)
    {
        return await _context.ShoppingLists
            .Include(sl => sl.ShoppingItems)
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }
    public async Task<IEnumerable<ShoppingList>> GetByGroupIdsAsync(int[] groupIds)
    {
        return await _context.ShoppingLists
            .Where(sl => groupIds.Contains(sl.GroupId))
            .ToListAsync();
    }
    public async Task<IEnumerable<ShoppingList>> GetByGroupIdAsync(int groupId)
    {
        return (IEnumerable<ShoppingList>)await _context.ShoppingLists
            .Include(sl => sl.ShoppingItems)
            .Where(sl => sl.GroupId == groupId)
            .ToListAsync();
    }
    public async Task AddAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Add(shoppingList);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(ShoppingList shoppingList)
    {
        _context.Entry(shoppingList).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(ShoppingList shoppingList)
    {
        _context.ShoppingLists.Remove(shoppingList);
        await _context.SaveChangesAsync();
    }
    public async Task<List<ShoppingList>> GetByCreatedByIdAsync(int createdById)
    {
        return await _context.ShoppingLists
            .Where(sl => sl.CreatedById == createdById)
            .ToListAsync();
    }
    public async Task<ShoppingList> GetByGuidAsync(Guid guid)
    {
        return await _context.ShoppingLists
            .FirstOrDefaultAsync(sl => sl.Guid == guid);
    }

    public async Task<bool> ExistsByGuidAsync(Guid guid)
    {
        return await _context.ShoppingLists
            .AnyAsync(sl => sl.Guid == guid);
    }

    public async Task<IEnumerable<ShoppingList>> GetByGroupGuidAsync(Guid groupGuid)
    {
        return await _context.ShoppingLists
            .Include(sl => sl.Group)
            .Where(sl => sl.Group.Guid == groupGuid)
            .ToListAsync();
    }
}
