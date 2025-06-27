using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyTracker.Database;
using MoneyTracker.DTOs;
using MoneyTracker.Models;
using MoneyTracker.Services.Email;
using MoneyTracker.Services.JwtToken;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;
        private readonly MoneyTrackerDbContext _context;

        public AuthController(IMapper mapper, ILogger<AuthController> logger,
            UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService, IEmailService emailService,
            MoneyTrackerDbContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid registration request.");
            }

            try
            {
                var user = _mapper.Map<User>(request);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                user.RefreshTokenExpiry = refreshTokenExpiry;
                user.RefreshToken = refreshToken;

                var resultCreateUser = await _userManager.CreateAsync(user, request.Password);
                var resultAddRole = await _userManager.AddToRoleAsync(user, SystemRole.USER);

                if (!resultCreateUser.Succeeded || !resultAddRole.Succeeded)
                {
                    _logger.LogError("User registration failed: {Errors}", resultCreateUser.Errors);
                    return StatusCode(500, "User registration failed.");
                }

                _logger.LogInformation("User registered successfully: {UserId}", user.Id);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // url to confirm email
                var callbackUrl = Url.Action(
                    action: "ConfirmEmail",
                    controller: "Auth",
                    values: new
                    {
                        userId = user.Id,
                        code = code
                    },
                    protocol: HttpContext.Request.Scheme
                );

                _logger.LogInformation("Email confirmation URL: {CallbackUrl}", callbackUrl);

                await _emailService.SendEmailByConfirmationEmailAsync(user.Email, callbackUrl);

                return Ok(new
                {
                    Message = "User registered successfully.",
                    Token = _jwtTokenService.GenerateToken(user, SystemRole.USER),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry,
                });

            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while registering the user.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid login request.");
            }

            var response = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (response.IsLockedOut)
            {
                _logger.LogWarning("User account is locked out: {Email}", request.Email);
                return StatusCode(423, "User account is locked out.");
            }

            if (!response.Succeeded)
            {
                _logger.LogWarning("Login failed for user: {Email}", request.Email);
                return Unauthorized("Invalid email or password.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var role = await _userManager.GetRolesAsync(user);

            user.RefreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return Ok(new
            {
                Message = "User logged in successfully.",
                Token = _jwtTokenService.GenerateToken(user, role.FirstOrDefault()),
                RefreshToken = user.RefreshToken.ToString(),
                RefreshTokenExpiry = user.RefreshTokenExpiry.ToString()
            });
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded)
                {
                    _logger.LogError("Email confirmation failed: {Errors}", result.Errors);
                    return StatusCode(500, "Email confirmation failed.");
                }

                _logger.LogInformation("Email confirmed successfully for user: {UserId}", user.Id);

                // Aqui podria ir la logica de redireccionamiento a una pagina de confirmacion
                return Ok(new { Message = "Email confirmed successfully." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while confirming the email.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User not found for logout: {UserId}", userId);
                return NotFound("User not found.");
            }

            await _signInManager.SignOutAsync();

            user.RefreshToken = null;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User logged out successfully: {UserId}", userId);
            
            return Ok(new
            {
                Message = "User logged out successfully.",
                UserId = userId
            });
        }

    }
}