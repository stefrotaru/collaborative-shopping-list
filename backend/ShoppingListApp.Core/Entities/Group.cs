public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Description { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User CreatedBy { get; set; }
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
}