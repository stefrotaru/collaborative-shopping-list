public interface IGroupRepository
{
    Task<Group> GetByIdAsync(int id);
    Task<IEnumerable<Group>> GetAllAsync();
    Task<IEnumerable<Group>> GetByUserIdAsync(int userId);
    Task AddAsync(Group group);
    Task UpdateAsync(Group group);
    Task DeleteAsync(Group group);
    Task AddUserToGroupAsync(GroupMember groupMember);
    Task RemoveUserFromGroupAsync(int groupId, int userId);

    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId);
    Task<IEnumerable<Group>> GetGroupMembershipsByUserIdAsync(int userId);

    Task<Group> GetByGuidAsync(Guid guid);
    Task<bool> ExistsByGuidAsync(Guid guid);

}
