using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ShoppingListApp.Core.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ILogger<CurrentUserService> logger = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _logger = logger;
        }

        public int GetCurrentUserId()
        {
            try
            {
                // Check if HttpContext is null
                if (_httpContextAccessor.HttpContext == null)
                {
                    Console.WriteLine("Warning: HttpContext is null in CurrentUserService.GetCurrentUserId()");
                    return 0;
                }

                // Get the token from the Authorization header
                var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    Console.WriteLine("Warning: No valid Authorization header found");
                    return 0;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Warning: Empty token in Authorization header");
                    return 0;
                }

                // Use your existing service to get user from token
                try
                {
                    var user = _userService.GetUserInfoAsync(token).Result;
                    if (user != null && user.Id > 0)
                    {
                        return user.Id;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting user from token: {ex.Message}");
                }

                return 0; // Default if not authenticated
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCurrentUserId: {ex.Message}");
                return 0;
            }
        }

        public string GetCurrentUserToken()
        {
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetCurrentUserToken");
                return null;
            }
        }
    }
}