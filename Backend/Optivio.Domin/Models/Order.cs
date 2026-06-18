using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Order : BaseEntity<int>
    {

        public string OrderNumber { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string Currency { get; set; }=null!;
        public DateTime? PaidAt { get; set; }
        public decimal Discount { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public string? CouponCode { get; set; }

        public decimal TotalsPrice { get; set; }
        public decimal ShippingCost { get; set; } 

        #region RelationShips
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        #endregion
    }

}
