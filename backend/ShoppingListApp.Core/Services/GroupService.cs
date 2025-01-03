public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GroupService(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GroupDto> CreateGroupAsync(string name, string description, int createdById)
    {
        // Retrieve the user who is creating the group
        var user = await _userRepository.GetByIdAsync(createdById);
        if (user == null)
        {
            throw new ArgumentException("Invalid user ID.");
        }

        // Create a new group entity
        var group = new Group
        {
            Name = name,
            Description = description,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };

        // Save the group to the database
        await _groupRepository.AddAsync(group);

        // Map the group entity to a DTO and return it
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        };
    }

    public async Task<GroupDto> GetGroupByIdAsync(int groupId)
    {
        // Retrieve the group by ID
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Map the group entity to a DTO and return it
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        };
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(int userId)
    {
        // Retrieve the groups associated with the user
        var groups = await _groupRepository.GetByUserIdAsync(userId);

        // Map the group entities to DTOs and return them
        return groups.Select(group => new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        });
    }

    public async Task UpdateGroupAsync(int groupId, string name, string description)
    {
        // Retrieve the group by ID
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Update the group properties
        group.Name = name;
        group.Description = description;

        // Save the changes to the database
        await _groupRepository.UpdateAsync(group);
    }

    public async Task DeleteGroupAsync(int groupId)
    {
        // Retrieve the group by ID
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Delete the group from the database
        await _groupRepository.DeleteAsync(group);
    }

    public async Task AddUserToGroupAsync(int groupId, int userId, string role)
    {
        // Check if the group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Check if the user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Create a new GroupMember entity
        var groupMember = new GroupMember
        {
            GroupId = groupId,
            UserId = userId,
            Role = role
        };

        // Add the user to the group
        await _groupRepository.AddUserToGroupAsync(groupMember);
    }

    public async Task RemoveUserFromGroupAsync(int groupId, int userId)
    {
        // Check if the group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Check if the user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Remove the user from the group
        await _groupRepository.RemoveUserFromGroupAsync(groupId, userId);
    }
}
