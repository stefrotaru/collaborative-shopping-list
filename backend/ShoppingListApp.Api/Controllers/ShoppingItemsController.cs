using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ShoppingItemsController : ControllerBase
{
    private readonly IShoppingItemService _shoppingItemService;
    private readonly IShoppingListService _shoppingListService;

    public ShoppingItemsController(
        IShoppingItemService shoppingItemService,
        IShoppingListService shoppingListService)
    {
        _shoppingItemService = shoppingItemService;
        _shoppingListService = shoppingListService;
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

    [HttpGet("shoppinglist/guid/{shoppingListGuid}")]
    public async Task<ActionResult<IEnumerable<ShoppingItemDto>>> GetByShoppingListGuid(Guid shoppingListGuid)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // First, get the shopping list by GUID
            var shoppingList = await _shoppingListService.GetShoppingListByGuidAsync(shoppingListGuid);

            // Then get the items using the shopping list's integer ID
            var shoppingItems = await _shoppingItemService.GetShoppingItemsByShoppingListIdAsync(shoppingList.Id);

            return Ok(shoppingItems);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving shopping items: {ex.Message}");
        }
    }

    [HttpPut("updateItem/{id}")]
    public async Task<IActionResult> Update(int id, string name, int quantity, bool isChecked)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingItemService.UpdateShoppingItemAsync(id, name, quantity, isChecked);
        return NoContent();
    }

    [HttpPut("updateItemChecked/{id}")]
    public async Task<IActionResult> UpdateItemChecked(int id, bool isChecked)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingItemService.UpdateShoppingItemCheckedAsync(id, isChecked);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _shoppingItemService.DeleteShoppingItemAsync(id);
        return Ok();
    }
}
