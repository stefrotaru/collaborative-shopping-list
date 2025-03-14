﻿using Microsoft.AspNetCore.Mvc;

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var group = await _groupService.GetGroupByIdAsync(id);
        return Ok(group);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetByUserId(int userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var groups = await _groupService.GetGroupsByUserIdAsync(userId);
        return Ok(groups);
    }

    [HttpGet("{groupId}/members")]
    public async Task<ActionResult<IEnumerable<GroupMemberDto>>> GetMembers(int groupId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var members = await _groupService.GetGroupMembersAsync(groupId);
            return Ok(members);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving group members." });
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
