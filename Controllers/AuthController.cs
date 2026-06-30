using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.DTOs;
using LogisticsManagementSystem.Enums;
using LogisticsManagementSystem.Models;
using LogisticsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogisticsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExists)
                return BadRequest("Email already exists.");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            var passwordHash = HashPassword(request.Password);

            if (user.PasswordHash != passwordHash)
                return Unauthorized("Invalid email or password.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new CurrentUserDto
            {
                Id = int.Parse(userId!),
                Email = email!,
                Role = role!
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending-drivers")]
        public async Task<IActionResult> GetPendingDrivers()
        {
            var pendingDrivers = await _context.Users
                .Where(u => u.Role == UserRole.Driver && u.Status == UserStatus.Pending)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Status,
                    u.CreatedDate
                })
                .ToListAsync();

            return Ok(pendingDrivers);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("approve-driver/{userId}")]
        public async Task<IActionResult> ApproveDriver(
    int userId,
    ApproveDriverRequestDto request)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            if (user.Role != UserRole.Driver)
                return BadRequest("Only Driver users can be approved as drivers.");

            if (user.Status != UserStatus.Pending)
                return BadRequest("Only pending drivers can be approved.");

            var driver = new Driver
            {
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = request.PhoneNumber,
                LicenseNumber = request.LicenseNumber,
                IsAvailable = request.IsAvailable,
                ApplicationUserId = user.Id
            };

            await _context.Drivers.AddAsync(driver);

            user.Status = UserStatus.Active;

            await _context.SaveChangesAsync();

            return Ok("Driver approved successfully.");
        }
    }
}