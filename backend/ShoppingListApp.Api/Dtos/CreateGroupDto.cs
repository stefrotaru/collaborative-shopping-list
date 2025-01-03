using System.ComponentModel.DataAnnotations;

public class CreateGroupDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Range(1, int.MaxValue)]
    public int CreatedById { get; set; }
}
