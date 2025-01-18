public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Avatar { get; set; }
    public string Token { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<GroupMember> GroupMembers { get; set; }
}
