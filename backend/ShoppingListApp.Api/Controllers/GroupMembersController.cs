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
    public async Task<IActionResult> AddUserToGroup(int groupId, int userId, string role)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _groupService.AddUserToGroupAsync(groupId, userId, role);
        return Ok();
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
