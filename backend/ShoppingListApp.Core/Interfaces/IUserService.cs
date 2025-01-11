public interface IUserService
{
    Task<UserDto> RegisterUserAsync(string username, string email, string password, string avatar);
    Task<UserDto> AuthenticateUserAsync(string email, string password);
    Task<UserDto> GetUserByIdAsync(int userId);
}
