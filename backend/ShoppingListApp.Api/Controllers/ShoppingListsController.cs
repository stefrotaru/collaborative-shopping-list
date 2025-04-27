using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ShoppingListApp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;
    private readonly ICurrentUserService _currentUserService;

    public ShoppingListsController(
        IShoppingListService shoppingListService,
        ICurrentUserService currentUserService)
    {
        _shoppingListService = shoppingListService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingListDto>> Create(CreateShoppingListDto createShoppingListDto)
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

        // Only allow the current user to create lists as themselves
        createShoppingListDto.CreatedById = currentUserId;

        var shoppingList = await _shoppingListService.CreateShoppingListAsync(
            createShoppingListDto.Name,
            createShoppingListDto.GroupId,
            createShoppingListDto.CreatedById);

        return CreatedAtAction(nameof(GetById), new { id = shoppingList.Id }, shoppingList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingListDto>> GetById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get current user
        int currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == 0)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        // Check if user has access to this shopping list
        var authService = HttpContext.RequestServices.GetRequiredService<IShoppingListAuthorizationService>();
        bool canAccess = await authService.CanAccessShoppingListAsync(currentUserId, id);

        if (!canAccess)
        {
            return Forbid();
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

        int currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == 0)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        try
        {
            // Check if the user is a member of the group
            var groupService = HttpContext.RequestServices.GetRequiredService<IGroupService>();
            var isGroupMember = await groupService.IsUserInGroupAsync(currentUserId, groupId);

            if (!isGroupMember)
            {
                return Unauthorized(new { message = "You don't have access to this group" });
                // Use Unauthorized instead of Forbid() if the authentication scheme isn't fully configured
            }

            var shoppingLists = await _shoppingListService.GetShoppingListsByGroupIdAsync(groupId);
            return Ok(shoppingLists);
        }
        catch (Exception ex)
        {
            // Log the error
            // _logger.LogError(ex, "Error in GetByGroupId for groupId {GroupId}", groupId);

            // Return a meaningful error response
            return StatusCode(500, new { message = "An error occurred while retrieving shopping lists", error = ex.Message });
        }
    }

    [HttpGet("allgroups")]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetByGroups([FromQuery(Name = "groupIds")] string groupIdsString)
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

        int[] groupIds = groupIdsString.Split(',').Select(int.Parse).ToArray();

        // Get group service to check membership
        var groupService = HttpContext.RequestServices.GetRequiredService<IGroupService>();
        var accessibleGroupIds = new List<int>();

        // Filter to only groups the user is a member of
        foreach (var groupId in groupIds)
        {
            var isGroupMember = await groupService.IsUserInGroupAsync(currentUserId, groupId);
            if (isGroupMember)
            {
                accessibleGroupIds.Add(groupId);
            }
        }

        var shoppingLists = await _shoppingListService.GetShoppingListsByGroupIdsAsync(accessibleGroupIds.ToArray());
        return Ok(shoppingLists);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateShoppingListDto updateShoppingListDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get current user
        int currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == 0)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        // Check if user has access to this shopping list
        var authService = HttpContext.RequestServices.GetRequiredService<IShoppingListAuthorizationService>();
        bool canAccess = await authService.CanAccessShoppingListAsync(currentUserId, id);

        if (!canAccess)
        {
            return Forbid();
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

        // Get current user
        int currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == 0)
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        // Check if user has access to this shopping list
        var authService = HttpContext.RequestServices.GetRequiredService<IShoppingListAuthorizationService>();
        bool canAccess = await authService.CanAccessShoppingListAsync(currentUserId, id);

        if (!canAccess)
        {
            return Forbid();
        }

        await _shoppingListService.DeleteShoppingListAsync(id);
        return NoContent();
    }

    [HttpGet("guid/{guid}")]
    public async Task<ActionResult<ShoppingListDto>> GetShoppingListByGuid(string guid)
    {
        try
        {
            if (!Guid.TryParse(guid, out var listGuid))
            {
                return BadRequest("Invalid GUID format.");
            }

            // Get current user
            int currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var shoppingList = await _shoppingListService.GetShoppingListByGuidAsync(listGuid);

            // Check authorization
            var authService = HttpContext.RequestServices.GetRequiredService<IShoppingListAuthorizationService>();
            bool canAccess = await authService.CanAccessShoppingListAsync(currentUserId, shoppingList.Id);

            if (!canAccess)
            {
                return Forbid();
            }


            return Ok(shoppingList);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("group/guid/{groupGuid}")]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetShoppingListsByGroupGuid(string groupGuid)
    {
        try
        {
            if (!Guid.TryParse(groupGuid, out var guid))
            {
                return BadRequest("Invalid GUID format.");
            }

            var shoppingLists = await _shoppingListService.GetShoppingListsByGroupGuidAsync(guid);
            return Ok(shoppingLists);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}