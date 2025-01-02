using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create(CreateGroupDto createGroupDto)
    {
        var group = await _groupService.CreateGroupAsync(createGroupDto.Name, createGroupDto.Description, createGroupDto.CreatedById);
        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDto>> GetById(int id)
    {
        var group = await _groupService.GetGroupByIdAsync(id);
        return Ok(group);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetByUserId(int userId)
    {
        var groups = await _groupService.GetGroupsByUserIdAsync(userId);
        return Ok(groups);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateGroupDto updateGroupDto)
    {
        await _groupService.UpdateGroupAsync(id, updateGroupDto.Name, updateGroupDto.Description);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _groupService.DeleteGroupAsync(id);
        return NoContent();
    }
}
