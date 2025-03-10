using System;

public class GroupMemberDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public string Role { get; set; } // To show if they're an admin or regular member
    public DateTime JoinedAt { get; set; }
}