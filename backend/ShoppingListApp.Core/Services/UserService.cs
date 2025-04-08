using ShoppingListApp.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<UserDto> RegisterUserAsync(string username, string email, string password, string avatar)
    {
        // Check if a user with the same email already exists
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        // Create a new user entity - we'll generate the token after saving
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Avatar = avatar,
            CreatedAt = DateTime.UtcNow
        };

        // Save the user to the database to get an ID
        await _userRepository.AddAsync(user);

        // Generate a token using the TokenService
        string token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        // Store the token for backward compatibility
        user.Token = token;
        await _userRepository.UpdateAsync(user);

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = token
        };
    }

    public async Task<UserDto> AuthenticateUserAsync(string email, string password)
    {
        // Retrieve the user by email
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new ArgumentException("Invalid email or password.");
        }

        // Verify the password
        if (!VerifyPassword(password, user.PasswordHash))
        {
            throw new ArgumentException("Invalid email or password.");
        }

        // Generate a fresh token on each login
        string token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);

        // Update the stored token for backward compatibility
        user.Token = token;
        await _userRepository.UpdateAsync(user);

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = token
        };
    }

    public async Task<UserDto> UpdateUserAsync(int userId, string username, string email, string avatar, string token)
    {
        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Check if email is being changed and if it's already in use by another user
        if (email != user.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null && existingUser.Id != userId)
            {
                throw new ArgumentException("A user with this email already exists.");
            }
        }

        // Update user properties
        user.Username = username;
        user.Email = email;
        user.Avatar = avatar;

        // Save the changes
        await _userRepository.UpdateAsync(user);

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = token // Use the provided token
        };
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Delete the user
        await _userRepository.DeleteAsync(user);

        return true;
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar
        };
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        // Retrieve the user by email
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new ArgumentException($"No user found with email: {email}");
        }

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = user.Token // Include if needed by your UserDto
        };
    }

    public async Task<UserDto> GetUserInfoAsync(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty");
            }

            // First try to validate the token with TokenService
            var userId = _tokenService.ValidateTokenAndGetUserId(token);

            if (userId.HasValue)
            {
                // Token is a valid JWT - get user by ID
                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Token = token // Return the same token
                };
            }

            // Fall back to legacy token lookup if JWT validation failed
            var userByToken = await _userRepository.GetByTokenAsync(token);
            if (userByToken == null)
            {
                throw new ArgumentException("Invalid token");
            }

            return new UserDto
            {
                Id = userByToken.Id,
                Username = userByToken.Username,
                Email = userByToken.Email,
                Avatar = userByToken.Avatar,
                Token = token
            };
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error in GetUserInfoAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<UserStatsDto> GetUserStatsAsync(string token)
    {
        // First, get the user ID from the token
        int userId;
        try
        {
            var tokenUserId = _tokenService.ValidateTokenAndGetUserId(token);
            if (!tokenUserId.HasValue)
            {
                // Fall back to legacy token lookup
                var user = await _userRepository.GetByTokenAsync(token);
                if (user == null)
                {
                    throw new ArgumentException("Invalid token");
                }
                userId = user.Id;
            }
            else
            {
                userId = tokenUserId.Value;
            }
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid token");
        }

        // Retrieve user stats from the repository using the correct method names
        var listsCount = await _userRepository.GetUserShoppingListsCountAsync(userId);
        var groupsCount = await _userRepository.GetUserGroupsCountAsync(userId);
        var itemsAddedCount = await _userRepository.GetUserItemsAddedCountAsync(userId);
        var itemsCompletedCount = await _userRepository.GetUserItemsCompletedCountAsync(userId);

        // Map the user stats to a DTO and return it
        return new UserStatsDto
        {
            UserId = userId.ToString(),
            TotalLists = listsCount,
            TotalGroups = groupsCount,
            TotalItemsAdded = itemsAddedCount,
            TotalItemsCompleted = itemsCompletedCount
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        // Retrieve the user by ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Verify the old password
        if (!VerifyPassword(oldPassword, user.PasswordHash))
        {
            throw new ArgumentException("Current password is incorrect.");
        }

        // Hash the new password
        user.PasswordHash = HashPassword(newPassword);

        // Save the changes
        await _userRepository.UpdateAsync(user);

        return true;
    }

    private string HashPassword(string password)
    {
        // TODO: Implement password hashing logic
        // You can use a library like BCrypt.Net for secure password hashing
        // For simplicity, we'll just return the password as is
        return password;
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        // TODO: Implement password verification logic
        // Compare the provided password with the hashed password
        // For simplicity, we'll just compare the strings directly
        return password == hashedPassword;
    }

    // This method is kept for backward compatibility but marked obsolete
    [Obsolete("Use TokenService.GenerateToken instead")]
    private string GenerateToken()
    {
        // Generate a unique token, e.g., using a GUID
        return Guid.NewGuid().ToString("N");
    }
}