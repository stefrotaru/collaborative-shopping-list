public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public ShoppingListService(IShoppingListRepository shoppingListRepository, IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _shoppingListRepository = shoppingListRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<ShoppingListDto> CreateShoppingListAsync(string name, int groupId, int createdById)
    {
        // Retrieve the group by ID
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
        {
            throw new ArgumentException("Invalid group ID.");
        }

        // Retrieve the user who is creating the shopping list
        var user = await _userRepository.GetByIdAsync(createdById);
        if (user == null)
        {
            throw new ArgumentException("Invalid user ID.");
        }

        // Create a new shopping list entity
        var shoppingList = new ShoppingList
        {
            Name = name,
            GroupId = groupId,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };

        // Save the shopping list to the database
        await _shoppingListRepository.AddAsync(shoppingList);

        // Map the shopping list entity to a DTO and return it
        return new ShoppingListDto
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            GroupId = shoppingList.GroupId
        };
    }

    public async Task<ShoppingListDto> GetShoppingListByIdAsync(int shoppingListId)
    {
        // Retrieve the shopping list by ID
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingList == null)
        {
            throw new ArgumentException("Shopping list not found.");
        }

        // Map the shopping list entity to a DTO and return it
        return new ShoppingListDto
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            GroupId = shoppingList.GroupId
        };
    }

    public async Task<IEnumerable<ShoppingListDto>> GetShoppingListsByGroupIdAsync(int groupId)
    {
        // Retrieve the shopping lists by group ID
        var shoppingLists = await _shoppingListRepository.GetByGroupIdAsync(groupId);

        // Map the shopping list entities to DTOs and return them
        return shoppingLists.Select(shoppingList => new ShoppingListDto
        {
            Id = shoppingList.Id,
            Name = shoppingList.Name,
            GroupId = shoppingList.GroupId
        });
    }

    public async Task UpdateShoppingListAsync(int shoppingListId, string name)
    {
        // Retrieve the shopping list by ID
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingList == null)
        {
            throw new ArgumentException("Shopping list not found.");
        }

        // Update the shopping list name
        shoppingList.Name = name;

        // Save the changes to the database
        await _shoppingListRepository.UpdateAsync(shoppingList);
    }

    public async Task DeleteShoppingListAsync(int shoppingListId)
    {
        // Retrieve the shopping list by ID
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingList == null)
        {
            throw new ArgumentException("Shopping list not found.");
        }

        // Delete the shopping list from the database
        await _shoppingListRepository.DeleteAsync(shoppingList);
    }
}
