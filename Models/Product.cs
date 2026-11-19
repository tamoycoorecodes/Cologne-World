using System.ComponentModel.DataAnnotations;

namespace CologneWorld.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Category { get; set; } = string.Empty;
    [Range(0.01, 100000000)]
    public decimal UnitPrice { get; set; }
    [StringLength(1000), Url]
    public string ImageUrl { get; set; } = string.Empty;
}
