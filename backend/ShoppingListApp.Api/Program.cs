using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using ShoppingListApp.Api.Hubs;
using ShoppingListApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Get JWT settings from configuration
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Check if JWT settings are present
bool useJwtAuth = !string.IsNullOrEmpty(jwtKey) &&
                  !string.IsNullOrEmpty(jwtIssuer) &&
                  !string.IsNullOrEmpty(jwtAudience);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Configure the database context
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ShoppingListContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
builder.Services.AddScoped<IShoppingItemRepository, ShoppingItemRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IShoppingItemService, ShoppingItemService>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();


// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add authorization services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IShoppingListAuthorizationService, ShoppingListAuthorizationService>();

builder.Services.AddControllers();


// Configure JWT authentication
if (useJwtAuth)
{
    // JWT Bearer Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Handle authentication failures with JSON responses
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/shoppinglisthub"))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                // Override the default challenge behavior
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "You are not authorized to access this resource",
                    statusCode = 401
                });

                await context.Response.WriteAsync(result);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "You do not have permission to access this resource",
                    statusCode = 403
                });

                await context.Response.WriteAsync(result);
            }
        };
    });
}
else
{
    // Simplified authentication for development
    // This will allow your current token-based auth to continue working
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Minimal JWT settings - allows any token through but still enforces auth middleware
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            SignatureValidator = (token, parameters) => new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token)
        };

        // Handle auth failures
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Custom token extraction from Authorization header
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                return Task.CompletedTask;
            }
        };
    });
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173" // My vue dev server
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Required for SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Apply CORS policy
app.UseCors();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ShoppingListHub>("/shoppinglisthub");
});

//app.MapControllers();
//app.MapHub<ShoppingListHub>("/shoppinglisthub");

//app.UseCors(policy =>
//    policy
//        .AllowAnyOrigin()
//        .AllowAnyMethod()
//        .AllowAnyHeader());

app.Run();