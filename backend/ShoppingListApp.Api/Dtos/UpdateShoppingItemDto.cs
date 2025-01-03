using System.ComponentModel.DataAnnotations;

public class UpdateShoppingItemDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public bool IsChecked { get; set; }
}
