public class CreateShoppingItemDto
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int ShoppingListId { get; set; }
    public int CreatedById { get; set; }
}
