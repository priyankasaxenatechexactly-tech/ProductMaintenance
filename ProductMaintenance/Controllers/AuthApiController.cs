using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.Models;
using System.Security.Claims;

namespace ProductMaintenance.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserProcess _userProcess;

        public AuthApiController(IUserProcess userProcess)
        {
            _userProcess = userProcess;
        }

        // POST: api/v1/auth/login (JSON)
        [HttpPost("login")]
        [Consumes("application/json")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LoginJson([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid request." });
            }

            var user = await _userProcess.UserLogin(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };
            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { id = user.Id, name = string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name, email = user.Email, role = user.Role });
        }

        // POST: api/v1/auth/login (form-data or x-www-form-urlencoded)
        [HttpPost("login")]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LoginForm([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid request." });
            }

            var user = await _userProcess.UserLogin(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };
            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { id = user.Id, name = string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name, email = user.Email, role = user.Role });
        }

        // POST: api/v1/auth/logout
        [HttpPost("logout")]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
