using CologneWorld.Data;
using CologneWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace CologneWorld.Services;

public class ProductService
{
    private readonly ApplicationDbContext _db;
    public ProductService(ApplicationDbContext db) => _db = db;

    // LINQ-to-Entities queries retrieve product records from the persistent SQLite database.
    public List<Product> GetProducts() => _db.Products.AsNoTracking().OrderBy(p => p.ProductId).ToList();
    public Product? GetProductById(int id) => _db.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
}
