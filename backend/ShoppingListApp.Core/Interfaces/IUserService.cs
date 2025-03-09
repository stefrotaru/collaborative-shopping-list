public interface IUserService
{
    Task<UserDto> RegisterUserAsync(string username, string email, string password, string avatar);
    Task<UserDto> AuthenticateUserAsync(string email, string password);
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> GetUserInfoAsync(string token);
    Task<UserDto> UpdateUserAsync(int userId, string username, string email, string avatar, string token);
}
