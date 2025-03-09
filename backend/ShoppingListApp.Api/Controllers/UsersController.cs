using Microsoft.AspNetCore.Authorization;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.RegisterUserAsync(registerUserDto.Username, registerUserDto.Email, registerUserDto.Password, registerUserDto.Avatar);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("update")]
    public async Task<ActionResult<UserDto>> Update(UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _userService.UpdateUserAsync(
                updateUserDto.Id,
                updateUserDto.Username,
                updateUserDto.Email,
                updateUserDto.Avatar,
                updateUserDto.Token);

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the user.");
        }
    }

    [HttpPut("changePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.ChangePasswordAsync(
                changePasswordDto.UserId,
                changePasswordDto.OldPassword,
                changePasswordDto.NewPassword);

            return Ok(new { message = "Password changed successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while changing the password." });
        }
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> Delete(DeleteUserDto deleteUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.DeleteUserAsync(deleteUserDto.Id);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<UserDto>> Authenticate(AuthenticateUserDto authenticateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.AuthenticateUserAsync(authenticateUserDto.Email, authenticateUserDto.Password);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }
    [HttpPost("userinfo")]
    //[Authorize] // TODO: configure authentication
    public async Task<ActionResult<UserDto>> GetUserInfo(GetUserInfoDto  getUserInfoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var token = getUserInfoDto.Token;
        var user = await _userService.GetUserInfoAsync(token);
        return Ok(user);
    }
}
