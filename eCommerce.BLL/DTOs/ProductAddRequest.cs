namespace eCommerce.BLL.DTOs;

public class ProductAddRequest
{
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public double? UnitPrice { get; set; }
    public int? QuantityInStock { get; set; }
}
