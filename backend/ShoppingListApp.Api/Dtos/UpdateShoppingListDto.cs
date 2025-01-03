using System.ComponentModel.DataAnnotations;

public class UpdateShoppingListDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}
