public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> RegisterUserAsync(string username, string email, string password, string avatar)
    {
        // Check if a user with the same email already exists
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        // Generate a unique token
        string token = GenerateToken();

        // Create a new user entity
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Avatar = avatar,
            Token = token,
            CreatedAt = DateTime.UtcNow
        };

        // Save the user to the database
        await _userRepository.AddAsync(user);

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = user.Token
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

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = user.Token
        };
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

    public async Task<UserDto> GetUserInfoAsync(string token)
    {
        // Find the user by token
        var user = await _userRepository.GetByTokenAsync(token);
        if (user == null)
        {
            throw new ArgumentException("Invalid token.");
        }

        // Map the user entity to a DTO and return it
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Avatar = user.Avatar,
            Token = user.Token
        };
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

    private string GenerateToken()
    {
        // Generate a unique token, e.g., using a GUID
        return Guid.NewGuid().ToString("N");
    }
}
