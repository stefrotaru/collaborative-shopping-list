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


        public async Task JoinGroupChannel(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group_{groupId}");
        }

        public async Task LeaveGroupChannel(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group_{groupId}");
        }
    }
}