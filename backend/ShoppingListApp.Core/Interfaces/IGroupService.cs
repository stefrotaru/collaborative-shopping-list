public interface IGroupService
{
    Task<GroupDto> CreateGroupAsync(string name, string description, int createdById);
    Task<GroupDto> GetGroupByIdAsync(int groupId);
    Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(int userId);
    Task UpdateGroupAsync(int groupId, string name, string description);
    Task DeleteGroupAsync(int groupId);
    Task AddUserToGroupAsync(int groupId, int userId, string role);
    Task RemoveUserFromGroupAsync(int groupId, int userId);
}
