using Microsoft.AspNetCore.Mvc;
using ShoppingListApp.Core.Services;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ICurrentUserService _currentUserService;

    public GroupsController(
        IGroupService groupService,
        ICurrentUserService currentUserService)
    {
        _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create(CreateGroupDto createGroupDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var group = await _groupService.CreateGroupAsync(createGroupDto.Name, createGroupDto.Description, createGroupDto.CreatedById);
        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDto>> GetById(int id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Safely get current user ID with null check
            int currentUserId = _currentUserService?.GetCurrentUserId() ?? 0;

            if (currentUserId == 0)
            {
                Console.WriteLine("Unable to get current user ID - returning Unauthorized");
                return Unauthorized(new { message = "User not authenticated" });
            }

            Console.WriteLine($"Retrieved currentUserId: {currentUserId}");

            // Additional logging to diagnose issues
            try
            {
                var isGroupMember = await _groupService.IsUserInGroupAsync(currentUserId, id);
                Console.WriteLine($"User {currentUserId} is member of group {id}: {isGroupMember}");

                if (!isGroupMember)
                {
                    return Unauthorized(new { message = "You don't have access to this group" });
                }

                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null)
                {
                    return NotFound(new { message = "Group not found" });
                }

                return Ok(group);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in group operations: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while accessing group data", error = ex.Message });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception in GetById: {ex}");
            return StatusCode(500, new { message = "An unexpected error occurred", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetByUserId(int userId, [FromQuery] bool includeAllAccessible = false)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            IEnumerable<GroupDto> groups;

            if (includeAllAccessible)
            {
                // Get both created groups and groups where user is a member
                groups = await _groupService.GetAllAccessibleGroupsByUserIdAsync(userId);
            }
            else
            {
                // Get only groups created by the user (original behavior)
                groups = await _groupService.GetGroupsByUserIdAsync(userId);
            }

            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred retrieving groups", error = ex.Message });
        }
    }

    [HttpGet("{groupId}/members")]
    public async Task<ActionResult<IEnumerable<GroupMemberDto>>> GetMembers(int groupId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Check if the user is a member of this group
            bool isGroupMember = await _groupService.IsUserInGroupAsync(currentUserId, groupId);
            if (!isGroupMember)
            {
                return Unauthorized(new { message = "You don't have access to this group" });
            }

            var members = await _groupService.GetGroupMembersAsync(groupId);
            return Ok(members);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the error

            // Return a meaningful error response
            return StatusCode(500, new { message = "An error occurred while retrieving group members", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateGroupDto updateGroupDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _groupService.UpdateGroupAsync(id, updateGroupDto.Name, updateGroupDto.Description);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _groupService.DeleteGroupAsync(id);
        return NoContent();
    }
}
