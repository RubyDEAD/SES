using Microsoft.AspNetCore.Mvc;
using SES.Models.ViewModels;
using SES.Services;

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

        // TODO: sign-in (create auth cookie/claims). For now just redirect:
        return Redirect(returnUrl ?? Url.Action("Index", "Students")!);
    }

    // Controllers/UsersController.cs (add to your existing controller)
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

}
