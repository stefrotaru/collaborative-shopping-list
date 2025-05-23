﻿public interface IGroupService
{
    Task<GroupDto> CreateGroupAsync(string name, string description, int createdById);
    Task<GroupDto> GetGroupByIdAsync(int groupId);
    Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(int userId);
    Task<IEnumerable<GroupDto>> GetAllAccessibleGroupsByUserIdAsync(int userId);
    Task UpdateGroupAsync(int groupId, string name, string description);
    Task DeleteGroupAsync(int groupId);
    Task AddUserToGroupAsync(int groupId, int userId, string role);
    Task RemoveUserFromGroupAsync(int groupId, int userId);

    Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId);
    Task<bool> IsUserInGroupAsync(int userId, int groupId);
    Task<bool> IsUserGroupAdminAsync(int userId, int groupId);

    Task<GroupDto> GetGroupByGuidAsync(Guid guid);
}
