public class ShoppingList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int GroupId { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Group Group { get; set; }
    public User CreatedBy { get; set; }
    public ICollection<ShoppingItem> ShoppingItems { get; set; } = new List<ShoppingItem>();
}