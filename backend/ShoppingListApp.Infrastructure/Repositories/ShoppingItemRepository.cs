using Microsoft.EntityFrameworkCore;

public class ShoppingItemRepository : IShoppingItemRepository
{
    private readonly ShoppingListContext _context;
    public ShoppingItemRepository(ShoppingListContext context)
    {
        _context = context;
    }
    public async Task<ShoppingItem> GetByIdAsync(int id)
    {
        return await _context.ShoppingItems.FindAsync(id);
    }
    public async Task<IEnumerable<ShoppingItem>> GetAllAsync()
    {
        return await _context.ShoppingItems.ToListAsync();
    }
    public async Task<IEnumerable<ShoppingItem>> GetByShoppingListIdAsync(int shoppingListId)
    {
        return await _context.ShoppingItems
            .Where(si => si.ShoppingListId == shoppingListId)
            .ToListAsync();
    }
    public async Task AddAsync(ShoppingItem shoppingItem)
    {
        _context.ShoppingItems.Add(shoppingItem);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(ShoppingItem shoppingItem)
    {
        _context.Entry(shoppingItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(ShoppingItem shoppingItem)
    {
        _context.ShoppingItems.Remove(shoppingItem);
        await _context.SaveChangesAsync();
    }
}
