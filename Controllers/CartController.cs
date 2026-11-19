using CologneWorld.Models;
using CologneWorld.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CologneWorld.Controllers;

public class CartController : Controller
{
    private readonly ProductService _productService;
    public CartController(ProductService productService) => _productService = productService;

    private ShoppingCart GetCart()
    {
        var json = HttpContext.Session.GetString("ShoppingCart");
        if (string.IsNullOrWhiteSpace(json)) return new ShoppingCart();
        try { return JsonSerializer.Deserialize<ShoppingCart>(json) ?? new ShoppingCart(); }
        catch (JsonException) { HttpContext.Session.Remove("ShoppingCart"); return new ShoppingCart(); }
    }

    private void SaveCart(ShoppingCart cart)
    {
        HttpContext.Session.SetString("ShoppingCart", JsonSerializer.Serialize(cart));
        HttpContext.Session.SetInt32("CartItemCount", cart.GetTotalItems());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        if (quantity < 1 || quantity > 10)
        {
            TempData["ErrorMessage"] = "Please choose a quantity between 1 and 10.";
            return RedirectToAction("Index", "Products");
        }
        var product = _productService.GetProductById(productId);
        if (product == null)
        {
            TempData["ErrorMessage"] = "That product could not be found.";
            return RedirectToAction("Index", "Products");
        }
        var cart = GetCart();
        cart.AddItem(new CartItem { ProductId = product.ProductId, ProductName = product.Name, Price = product.UnitPrice, Quantity = quantity });
        SaveCart(cart);
        TempData["SuccessMessage"] = $"{product.Name} added to your cart.";
        return RedirectToAction("Index", "Products");
    }

    public IActionResult Index() => View(GetCart());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart(); cart.RemoveItem(productId); SaveCart(cart);
        TempData["SuccessMessage"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 1 || quantity > 10)
        {
            TempData["ErrorMessage"] = "Quantity must be between 1 and 10.";
            return RedirectToAction(nameof(Index));
        }
        var cart = GetCart(); cart.UpdateQuantity(productId, quantity); SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        var cart = GetCart(); cart.ClearCart(); SaveCart(cart);
        TempData["SuccessMessage"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));
    }
}
