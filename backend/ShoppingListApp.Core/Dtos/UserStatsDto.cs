using System.ComponentModel.DataAnnotations;

public class UserStatsDto
{
    public required string UserId { get; set; }
    public int TotalLists { get; set; }
    public int TotalGroups { get; set; }
    public int TotalItemsAdded { get; set; }
    public int TotalItemsCompleted { get; set; }
    //public int TotalSharedLists { get; set; }
    //public int TotalSharedUsers { get; set; }
    //public DateTime LastLoginDate { get; set; }
    //public DateTime AccountCreationDate { get; set; }
}

