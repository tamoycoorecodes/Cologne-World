using CologneWorld.Data;
using CologneWorld.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CologneWorld.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    public AdminController(ApplicationDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        return View(products);
    }


    [HttpGet("Users")]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var userRoles = new Dictionary<string, IList<string>>();
        foreach (var user in users)
            userRoles[user.Id] = await _userManager.GetRolesAsync(user);
        ViewBag.UserRoles = userRoles;
        return View(users);
    }

    [HttpPost("Users/SetRole"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserRole(string userId, string role)
    {
        if (role != "Admin" && role != "Customer") return BadRequest();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join("; ", removeResult.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Users));
        }
        var addResult = await _userManager.AddToRoleAsync(user, role);
        TempData[addResult.Succeeded ? "SuccessMessage" : "ErrorMessage"] = addResult.Succeeded
            ? $"Role updated for {user.Email}."
            : string.Join("; ", addResult.Errors.Select(e => e.Description));
        return RedirectToAction(nameof(Users));
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new Product());

    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Product added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        return product == null ? NotFound() : View(product);
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId) return BadRequest();
        if (!ModelState.IsValid) return View(product);
        var existing = await _db.Products.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Category = product.Category;
        existing.UnitPrice = product.UnitPrice;
        existing.ImageUrl = product.ImageUrl;
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
