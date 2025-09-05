using Microsoft.AspNetCore.Mvc;
using SES.Models;
using SES.Models.ViewModels;
using SES.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class UsersController : Controller
{
    private readonly IUserService _users;
    public UsersController(IUserService users) => _users = users;

    [HttpGet]
    public IActionResult Login() => View(new LoginVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var (ok, error, user) = await _users.LoginAsync(vm.Email, vm.Password);
        if (!ok || user == null)
        {
            ModelState.AddModelError(string.Empty, error ?? "Invalid credentials.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("StudentId", user.StudentId?.ToString() ?? ""),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (user.Role == UserRole.Admin)
        {
            return RedirectToAction("AdminIndex", "Courses");
        }

        return RedirectToAction("Profile", "Students", new { id = user.StudentId });
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var (ok, error) = await _users.RegisterAsync(vm.Email, vm.FirstName, vm.LastName, vm.Password);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Registration failed.");
            return View(vm);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
