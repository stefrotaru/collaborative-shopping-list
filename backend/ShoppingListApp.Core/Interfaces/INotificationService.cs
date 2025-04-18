public interface INotificationService
{
    Task NotifyShoppingListUpdated(int listId, string updatedById);
    Task NotifyShoppingItemAdded(int listId, int itemId, string addedById);
    Task NotifyShoppingItemUpdated(int listId, int itemId, string updatedById);
    Task NotifyShoppingItemRemoved(int listId, int itemId, string removedById);

    Task NotifyUserAddedToGroup(int groupId, int userId, string addedBy);
    Task NotifyUserRemovedFromGroup(int groupId, int userId, string removedBy);
}