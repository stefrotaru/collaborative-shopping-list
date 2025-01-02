public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public ICollection<GroupMember> GroupMembers { get; set; }
    public ICollection<ShoppingList> ShoppingLists { get; set; }
}
