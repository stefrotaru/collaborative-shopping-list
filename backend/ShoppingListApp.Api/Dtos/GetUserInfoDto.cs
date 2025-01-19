using System.ComponentModel.DataAnnotations;

public class GetUserInfoDto
{
    [Required]
    public string Token { get; set; }
}
