public class GroupMember
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } // "admin" or "member"
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public Group Group { get; set; }
    public User User { get; set; }
}