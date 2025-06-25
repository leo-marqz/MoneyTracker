using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyTracker.DTOs;
using MoneyTracker.Models;
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

        public AuthController(IMapper mapper, ILogger<AuthController> logger,
            UserManager<User> userManager, SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
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

                return Ok(new
                {
                    Message = "User registered successfully.",
                    Token = _jwtTokenService.GenerateToken(user),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshTokenExpiry,
                    User = user
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