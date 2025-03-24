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

        // Add the creator as a member and admin of the group
        await AddUserToGroupAsync(group.Id, createdById, "Admin");

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
        // Retrieve the groups created by the user
        var groups = await _groupRepository.GetByUserIdAsync(userId);

        // Map the group entities to DTOs and return them
        return groups.Select(group => new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedById = group.CreatedById
        }).ToList();
    }

    public async Task<IEnumerable<GroupDto>> GetAllAccessibleGroupsByUserIdAsync(int userId)
    {
        // First, get groups created by the user
        var createdGroups = await GetGroupsByUserIdAsync(userId);

        // Then, get groups where the user is a member
        var groupMemberships = await _groupRepository.GetGroupMembershipsByUserIdAsync(userId);
        var memberGroups = new List<GroupDto>();

        foreach (var membership in groupMemberships)
        {
            // Skip groups the user created (to avoid duplicates)
            var group = await _groupRepository.GetByIdAsync(membership.Id);
            if (group != null && group.CreatedById != userId)
            {
                memberGroups.Add(new GroupDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    CreatedById = group.CreatedById
                    // Add any other properties your GroupDto has
                });
            }
        }

        // Combine both lists and return
        return createdGroups.Concat(memberGroups);
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

    public async Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId)
    {
        // Check if the group exists
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found.");
        }

        // Get all members of the group, including their user information
        var groupMembers = await _groupRepository.GetGroupMembersAsync(groupId);

        // Map to DTOs
        var memberDtos = groupMembers.Select(member => new GroupMemberDto
        {
            UserId = member.UserId,
            Username = member.User.Username,
            Email = member.User.Email,
            Avatar = member.User.Avatar,
            Role = member.Role,
            JoinedAt = member.JoinedAt
        }).ToList();

        return memberDtos;
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
    public async Task<bool> IsUserInGroupAsync(int userId, int groupId)
    {
        try
        {
            Console.WriteLine($"Checking membership for userId {userId} in groupId {groupId}");

            // Get members explicitly for debugging
            var members = await _groupRepository.GetGroupMembersAsync(groupId);

            if (members == null || !members.Any())
            {
                Console.WriteLine($"No members found for groupId {groupId}");
                return false;
            }

            // Log each member for debugging
            foreach (var member in members)
            {
                Console.WriteLine($"Group {groupId} member: {member.UserId}, Role: {member.Role}");
            }

            bool isMember = members.Any(m => m.UserId == userId);
            Console.WriteLine($"User {userId} is member of group {groupId}: {isMember}");

            return isMember;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in IsUserInGroupAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsUserGroupAdminAsync(int userId, int groupId)
    {
        var groupMembers = await _groupRepository.GetGroupMembersAsync(groupId);
        var member = groupMembers.FirstOrDefault(m => m.UserId == userId);
        return member != null && member.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}
