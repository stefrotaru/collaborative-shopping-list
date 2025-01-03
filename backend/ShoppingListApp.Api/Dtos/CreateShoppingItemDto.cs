using System.ComponentModel.DataAnnotations;

public class CreateShoppingItemDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ShoppingListId { get; set; }

    [Range(1, int.MaxValue)]
    public int CreatedById { get; set; }
}
