using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto registerUserDto)
    {
        var user = await _userService.RegisterUserAsync(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<UserDto>> Authenticate(AuthenticateUserDto authenticateUserDto)
    {
        var user = await _userService.AuthenticateUserAsync(authenticateUserDto.Email, authenticateUserDto.Password);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }
}
