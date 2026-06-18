using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Shared.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalsPrice { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
