public class ShoppingItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public bool IsChecked { get; set; }
    public int ShoppingListId { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ShoppingList ShoppingList { get; set; }
    public User CreatedBy { get; set; }
}