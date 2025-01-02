public class ShoppingList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
    public ICollection<ShoppingItem> ShoppingItems { get; set; }
}
