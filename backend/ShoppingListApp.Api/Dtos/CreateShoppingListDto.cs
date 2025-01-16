using System.ComponentModel.DataAnnotations;

public class CreateShoppingListDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(1, int.MaxValue)]
    public int GroupId { get; set; }

    [Range(1, int.MaxValue)]
    public int CreatedById { get; set; }
}
