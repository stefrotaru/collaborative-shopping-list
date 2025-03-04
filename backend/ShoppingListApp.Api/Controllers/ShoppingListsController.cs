﻿using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingListDto>> Create(CreateShoppingListDto createShoppingListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingList = await _shoppingListService.CreateShoppingListAsync(createShoppingListDto.Name, createShoppingListDto.GroupId, createShoppingListDto.CreatedById);
        return CreatedAtAction(nameof(GetById), new { id = shoppingList.Id }, shoppingList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingListDto>> GetById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id);
        return Ok(shoppingList);
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetByGroupId(int groupId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingLists = await _shoppingListService.GetShoppingListsByGroupIdAsync(groupId);
        return Ok(shoppingLists);
    }

    [HttpGet("allgroups")]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetByGroups([FromQuery(Name = "groupIds")] string groupIdsString)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int[] groupIds = groupIdsString.Split(',').Select(int.Parse).ToArray();
        var shoppingLists = await _shoppingListService.GetShoppingListsByGroupIdsAsync(groupIds);
        return Ok(shoppingLists);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateShoppingListDto updateShoppingListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingListService.UpdateShoppingListAsync(id, updateShoppingListDto.Name);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingListService.DeleteShoppingListAsync(id);
        return NoContent();
    }
}
