using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }
}
