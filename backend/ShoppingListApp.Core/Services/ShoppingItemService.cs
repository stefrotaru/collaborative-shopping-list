﻿public class ShoppingItemService : IShoppingItemService
{
    private readonly IShoppingItemRepository _shoppingItemRepository;
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IUserRepository _userRepository;

    public ShoppingItemService(IShoppingItemRepository shoppingItemRepository, IShoppingListRepository shoppingListRepository, IUserRepository userRepository)
    {
        _shoppingItemRepository = shoppingItemRepository;
        _shoppingListRepository = shoppingListRepository;
        _userRepository = userRepository;
    }

    public async Task<ShoppingItemDto> AddShoppingItemAsync(string name, int quantity, int shoppingListId, int createdById)
    {
        // Retrieve the shopping list by ID
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingList == null)
        {
            throw new ArgumentException("Invalid shopping list ID.");
        }

        // Retrieve the user who is adding the shopping item
        var user = await _userRepository.GetByIdAsync(createdById);
        if (user == null)
        {
            throw new ArgumentException("Invalid user ID.");
        }

        // Create a new shopping item entity
        var shoppingItem = new ShoppingItem
        {
            Name = name,
            Quantity = quantity,
            IsChecked = false,
            ShoppingListId = shoppingListId,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };

        // Save the shopping item to the database
        await _shoppingItemRepository.AddAsync(shoppingItem);

        // Map the shopping item entity to a DTO and return it
        return new ShoppingItemDto
        {
            Id = shoppingItem.Id,
            Name = shoppingItem.Name,
            Quantity = shoppingItem.Quantity,
            IsChecked = shoppingItem.IsChecked,
            ShoppingListId = shoppingItem.ShoppingListId
        };
    }

    public async Task<ShoppingItemDto> GetShoppingItemByIdAsync(int shoppingItemId)
    {
        // Retrieve the shopping item by ID
        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(shoppingItemId);
        if (shoppingItem == null)
        {
            throw new ArgumentException("Shopping item not found.");
        }

        // Map the shopping item entity to a DTO and return it
        return new ShoppingItemDto
        {
            Id = shoppingItem.Id,
            Name = shoppingItem.Name,
            Quantity = shoppingItem.Quantity,
            IsChecked = shoppingItem.IsChecked,
            ShoppingListId = shoppingItem.ShoppingListId
        };
    }

    //Task UpdateShoppingItemAsync(int shoppingItemId, string name, int quantity, bool isChecked);
    //Task DeleteShoppingItemAsync(int shoppingItemId);

    public async Task<IEnumerable<ShoppingItemDto>> GetShoppingItemsByShoppingListIdAsync(int shoppingListId)
    {
        // Retrieve the shopping items by shopping list ID
        var shoppingItems = await _shoppingItemRepository.GetByShoppingListIdAsync(shoppingListId);

        // Map the shopping item entities to DTOs and return them
        return shoppingItems.Select(shoppingItem => new ShoppingItemDto
        {
            Id = shoppingItem.Id,
            Name = shoppingItem.Name,
            Quantity = shoppingItem.Quantity,
            IsChecked = shoppingItem.IsChecked,
            ShoppingListId = shoppingItem.ShoppingListId
        });
    }
    public async Task UpdateShoppingItemAsync(int shoppingItemId, string name, int quantity, bool isChecked)
    {
        // Retrieve the shopping item by ID
        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(shoppingItemId);
        if (shoppingItem == null)
        {
            throw new ArgumentException("Shopping item not found.");
        }

        // Update the shopping item properties
        shoppingItem.Name = name;
        shoppingItem.Quantity = quantity;
        shoppingItem.IsChecked = isChecked;

        // Save the changes to the database
        await _shoppingItemRepository.UpdateAsync(shoppingItem);
    }

    public async Task DeleteShoppingItemAsync(int shoppingItemId)
    {
        // Retrieve the shopping item by ID
        var shoppingItem = await _shoppingItemRepository.GetByIdAsync(shoppingItemId);
        if (shoppingItem == null)
        {
            throw new ArgumentException("Shopping item not found.");
        }

        // Delete the shopping item from the database
        await _shoppingItemRepository.DeleteAsync(shoppingItem);
    }
}