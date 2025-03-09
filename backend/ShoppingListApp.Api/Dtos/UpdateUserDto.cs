using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    public string Avatar { get; set; }

    public string Token { get; set; }
}