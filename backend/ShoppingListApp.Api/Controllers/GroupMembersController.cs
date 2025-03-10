using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GroupMembersController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupMembersController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToGroup([FromBody] AddGroupMemberDto addGroupMemberDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _groupService.AddUserToGroupAsync(
                addGroupMemberDto.GroupId,
                addGroupMemberDto.UserId,
                addGroupMemberDto.Role);

            return Ok(new { message = "User added to group successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while adding the user to the group." });
        }
    }

    [HttpDelete("{groupId}/{userId}")]
    public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _groupService.RemoveUserFromGroupAsync(groupId, userId);
        return Ok();
    }
}
