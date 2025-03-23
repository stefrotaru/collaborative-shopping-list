public class AccessibleShoppingListDto
{
    public int ShoppingListId { get; set; }
    public string ShoppingListName { get; set; }
    public int? GroupId { get; set; }
    public string GroupName { get; set; }
    public string AccessReason { get; set; } // e.g., "Owner", "Group Member"
}