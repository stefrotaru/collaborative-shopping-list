using System.ComponentModel.DataAnnotations;

public class ChangePasswordDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }
}
