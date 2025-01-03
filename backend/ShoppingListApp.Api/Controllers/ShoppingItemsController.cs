﻿using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ShoppingItemsController : ControllerBase
{
    private readonly IShoppingItemService _shoppingItemService;

    public ShoppingItemsController(IShoppingItemService shoppingItemService)
    {
        _shoppingItemService = shoppingItemService;
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingItemDto>> Create(CreateShoppingItemDto createShoppingItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingItem = await _shoppingItemService.AddShoppingItemAsync(createShoppingItemDto.Name, createShoppingItemDto.Quantity, createShoppingItemDto.ShoppingListId, createShoppingItemDto.CreatedById);
        return CreatedAtAction(nameof(GetById), new { id = shoppingItem.Id }, shoppingItem);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingItemDto>> GetById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingItem = await _shoppingItemService.GetShoppingItemByIdAsync(id);
        return Ok(shoppingItem);
    }

    [HttpGet("shoppinglist/{shoppingListId}")]
    public async Task<ActionResult<IEnumerable<ShoppingItemDto>>> GetByShoppingListId(int shoppingListId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var shoppingItems = await _shoppingItemService.GetShoppingItemsByShoppingListIdAsync(shoppingListId);
        return Ok(shoppingItems);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateShoppingItemDto updateShoppingItemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingItemService.UpdateShoppingItemAsync(id, updateShoppingItemDto.Name, updateShoppingItemDto.Quantity, updateShoppingItemDto.IsChecked);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingItemService.DeleteShoppingItemAsync(id);
        return NoContent();
    }
}
