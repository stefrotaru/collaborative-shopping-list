public interface IGroupRepository
{
    Task<Group> GetByIdAsync(int id);
    Task<IEnumerable<Group>> GetAllAsync();
    Task<IEnumerable<Group>> GetByUserIdAsync(int userId);
    Task AddAsync(Group group);
    Task UpdateAsync(Group group);
    Task DeleteAsync(Group group);
}
