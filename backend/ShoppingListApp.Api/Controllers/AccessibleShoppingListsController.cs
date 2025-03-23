using Microsoft.AspNetCore.Mvc;
using ShoppingListApp.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessibleShoppingListsController : ControllerBase
    {
        private readonly IShoppingListAuthorizationService _authorizationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IShoppingListService _shoppingListService;

        public AccessibleShoppingListsController(
            IShoppingListAuthorizationService authorizationService,
            ICurrentUserService currentUserService,
            IShoppingListService shoppingListService)
        {
            _authorizationService = authorizationService;
            _currentUserService = currentUserService;
            _shoppingListService = shoppingListService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessibleShoppingListDto>>> GetAccessibleShoppingLists()
        {
            int currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var accessibleLists = await _authorizationService.GetAccessibleShoppingListsAsync(currentUserId);
            return Ok(accessibleLists);
        }

        [HttpGet("can-access/{id}")]
        public async Task<ActionResult<bool>> CanAccessShoppingList(int id)
        {
            int currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            bool canAccess = await _authorizationService.CanAccessShoppingListAsync(currentUserId, id);
            return Ok(new { canAccess });
        }

        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetAccessibleShoppingListDetails()
        {
            int currentUserId = _currentUserService.GetCurrentUserId();
            if (currentUserId == 0)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var accessibleLists = await _authorizationService.GetAccessibleShoppingListsAsync(currentUserId);
            var shoppingListIds = accessibleLists.Select(al => al.ShoppingListId).ToList();

            // Get full details for each shopping list
            var result = new List<ShoppingListDto>();
            foreach (var id in shoppingListIds)
            {
                var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id);
                if (shoppingList != null)
                {
                    result.Add(shoppingList);
                }
            }

            return Ok(result);
        }
    }
}