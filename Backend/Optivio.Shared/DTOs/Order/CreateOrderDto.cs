using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Shared.DTOs.Order
{
    public class CreateOrderDto
    {
        public string Currency { get; set; } = null!;
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
