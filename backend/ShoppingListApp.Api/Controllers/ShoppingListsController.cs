using Microsoft.AspNetCore.Mvc;

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
        var shoppingList = await _shoppingListService.CreateShoppingListAsync(createShoppingListDto.Name, createShoppingListDto.GroupId, createShoppingListDto.CreatedById);
        return CreatedAtAction(nameof(GetById), new { id = shoppingList.Id }, shoppingList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingListDto>> GetById(int id)
    {
        var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id);
        return Ok(shoppingList);
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetByGroupId(int groupId)
    {
        var shoppingLists = await _shoppingListService.GetShoppingListsByGroupIdAsync(groupId);
        return Ok(shoppingLists);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateShoppingListDto updateShoppingListDto)
    {
        await _shoppingListService.UpdateShoppingListAsync(id, updateShoppingListDto.Name);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _shoppingListService.DeleteShoppingListAsync(id);
        return NoContent();
    }
}
