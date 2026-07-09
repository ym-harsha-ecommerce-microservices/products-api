using eCommerce.DAL.Entities;

namespace eCommerce.BLL.DTOs;

public class ProductUpdateRequest
{
    public Guid ProductID { get; set; }
    public string? ProductName { get; set; }
    public CategoryOptions Category { get; set; }
    public double? UnitPrice { get; set; }
    public int? QuantityInStock { get; set; }
}
