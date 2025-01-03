using System.ComponentModel.DataAnnotations;

public class CreateShoppingListDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public int GroupId { get; set; }

    [Range(1, int.MaxValue)]
    public int CreatedById { get; set; }
}
