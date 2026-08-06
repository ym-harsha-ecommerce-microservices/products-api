using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.BLL.RabbitMQ.ProductMessages;

public class ProductNameUpdateMessage
{
    public Guid ProductId { get; set; }
    public string ProductNewName { get; set; } = string.Empty;
}
