using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ShoppingListApp.Api.Hubs
{
    public class ShoppingListHub : Hub
    {
        public async Task JoinShoppingListGroup(int listId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"shoppingList_{listId}");
        }

        public async Task LeaveShoppingListGroup(int listId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"shoppingList_{listId}");
        }
    }
}