using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShoppingListContext>
{
    public ShoppingListContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShoppingListContext>();
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=CollaborativeShoppingListDb;Trusted_Connection=True;TrustServerCertificate=True");

        return new ShoppingListContext(optionsBuilder.Options);
    }
}