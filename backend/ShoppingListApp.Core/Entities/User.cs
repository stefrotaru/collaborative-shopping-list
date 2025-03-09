public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Avatar { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    public ICollection<ShoppingList> CreatedShoppingLists { get; set; } = new List<ShoppingList>();
    public ICollection<ShoppingItem> CreatedShoppingItems { get; set; } = new List<ShoppingItem>();
}