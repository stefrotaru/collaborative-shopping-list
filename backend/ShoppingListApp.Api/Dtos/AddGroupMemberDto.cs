using System.ComponentModel.DataAnnotations;

public class AddGroupMemberDto
{
    [Required]
    public int GroupId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; }
}