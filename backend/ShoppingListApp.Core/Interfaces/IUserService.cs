public interface IUserService
{
    Task<UserDto> RegisterUserAsync(string username, string email, string password);
    Task<UserDto> AuthenticateUserAsync(string email, string password);
    Task<UserDto> GetUserByIdAsync(int userId);
}
