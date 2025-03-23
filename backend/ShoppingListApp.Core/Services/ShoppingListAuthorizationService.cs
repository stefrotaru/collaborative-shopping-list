using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShoppingListApp.Core.Services
{
    public class ShoppingListAuthorizationService : IShoppingListAuthorizationService
    {
        private readonly IShoppingListService _shoppingListService;
        private readonly IGroupService _groupService;
        private readonly ILogger<ShoppingListAuthorizationService> _logger;

        public ShoppingListAuthorizationService(
            IShoppingListService shoppingListService,
            IGroupService groupService,
            ILogger<ShoppingListAuthorizationService> logger = null)
        {
            _shoppingListService = shoppingListService;
            _groupService = groupService;
            _logger = logger;
        }

        public async Task<bool> CanAccessShoppingListAsync(int userId, int shoppingListId)
        {
            try
            {
                // Get the shopping list details
                var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(shoppingListId);
                if (shoppingList == null)
                {
                    _logger?.LogWarning("Shopping list {ShoppingListId} not found during access check", shoppingListId);
                    return false;
                }

                // Log the details for debugging
                _logger?.LogInformation(
                    "Checking access for User {UserId} to List {ListId} created by {CreatedById} in Group {GroupId}",
                    userId, shoppingListId, shoppingList.CreatedById, shoppingList.GroupId);

                // Check if the user is the creator of the shopping list
                // This is the main check that should work for owners
                if (shoppingList.CreatedById == userId)
                {
                    _logger?.LogInformation("Access granted: User {UserId} is the creator of list {ListId}", userId, shoppingListId);
                    return true;
                }

                // Check if the user is a member of the group that the shopping list belongs to
                if (shoppingList.GroupId != 0) // Assuming 0 means no group
                {
                    var isGroupMember = await _groupService.IsUserInGroupAsync(userId, shoppingList.GroupId);
                    if (isGroupMember)
                    {
                        _logger?.LogInformation(
                            "Access granted: User {UserId} is a member of group {GroupId} that contains list {ListId}",
                            userId, shoppingList.GroupId, shoppingListId);
                        return true;
                    }
                }

                _logger?.LogWarning(
                    "Access denied: User {UserId} has no access to list {ListId} in group {GroupId}",
                    userId, shoppingListId, shoppingList.GroupId);
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error checking access for User {UserId} to Shopping List {ShoppingListId}", userId, shoppingListId);
                return false;
            }
        }

        public async Task<List<AccessibleShoppingListDto>> GetAccessibleShoppingListsAsync(int userId)
        {
            var result = new List<AccessibleShoppingListDto>();

            try
            {
                // Get all lists created by the user
                var ownedLists = await _shoppingListService.GetShoppingListsByCreatedByIdAsync(userId);

                // Get all groups the user is a member of
                var userGroups = await _groupService.GetGroupsByUserIdAsync(userId);

                // Add owned lists to the result
                foreach (var list in ownedLists)
                {
                    string groupName = null;
                    if (list.GroupId != 0)
                    {
                        var group = userGroups.FirstOrDefault(g => g.Id == list.GroupId);
                        groupName = group?.Name;
                    }

                    result.Add(new AccessibleShoppingListDto
                    {
                        ShoppingListId = list.Id,
                        ShoppingListName = list.Name,
                        GroupId = list.GroupId,
                        GroupName = groupName ?? "Personal",
                        AccessReason = "Owner"
                    });

                    _logger?.LogInformation("Added owned list {ListId} to accessible lists for user {UserId}", list.Id, userId);
                }

                // Add shared lists that the user doesn't own
                foreach (var group in userGroups)
                {
                    var groupLists = await _shoppingListService.GetShoppingListsByGroupIdAsync(group.Id);

                    foreach (var list in groupLists)
                    {
                        // Skip lists that the user owns (already added)
                        if (list.CreatedById == userId)
                            continue;

                        // Skip duplicates
                        if (result.Any(r => r.ShoppingListId == list.Id))
                            continue;

                        result.Add(new AccessibleShoppingListDto
                        {
                            ShoppingListId = list.Id,
                            ShoppingListName = list.Name,
                            GroupId = list.GroupId,
                            GroupName = group.Name,
                            AccessReason = $"Member of {group.Name}"
                        });

                        _logger?.LogInformation("Added shared list {ListId} to accessible lists for user {UserId} via group {GroupId}",
                            list.Id, userId, group.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting accessible shopping lists for user {UserId}", userId);
            }

            return result;
        }
    }
}