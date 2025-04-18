using Microsoft.AspNetCore.SignalR;
using ShoppingListApp.Api.Hubs;

namespace ShoppingListApp.Api.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<ShoppingListHub> _hubContext;

        public SignalRNotificationService(IHubContext<ShoppingListHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyShoppingListUpdated(int listId, string updatedBy)
        {
            await _hubContext.Clients.Group($"shoppingList_{listId}")
                .SendAsync("ShoppingListUpdated", listId, updatedBy);
        }

        public async Task NotifyShoppingItemAdded(int listId, int itemId, string addedById)
        {
            await _hubContext.Clients.Group($"shoppingList_{listId}")
                .SendAsync("ShoppingItemAdded", listId, itemId, addedById);
        }

        public async Task NotifyShoppingItemUpdated(int listId, int itemId, string updatedById)
        {
            await _hubContext.Clients.Group($"shoppingList_{listId}")
                .SendAsync("ShoppingItemUpdated", listId, itemId, updatedById);
        }

        public async Task NotifyShoppingItemRemoved(int listId, int itemId, string removedById)
        {
            await _hubContext.Clients.Group($"shoppingList_{listId}")
                .SendAsync("ShoppingItemRemoved", listId, itemId, removedById);
        }
    }
}