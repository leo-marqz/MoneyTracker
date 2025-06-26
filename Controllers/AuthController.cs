using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public AuthController(IMapper mapper, ILogger<AuthController> logger,
            UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService, IEmailService emailService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _emailService = emailService;
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

                if(!resultCreateUser.Succeeded || !resultAddRole.Succeeded)
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
                    values: new {
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
            return Ok(new { Message = "User logged in successfully." });
        }

        private string GenerateJwtToken(User user)
        {
            // This method should generate a JWT token for the user
            // For now, we will return a placeholder string
            return "GeneratedJWTToken";
        }

        private string RefreshJwtToken(string token)
        {
            // This method should refresh the JWT token
            // For now, we will return a placeholder string
            return "RefreshedJWTToken";
        }

    }
}