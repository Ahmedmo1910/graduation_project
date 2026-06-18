using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Payment : BaseEntity<int>
    {

        //public string Provider { get; set; } = null!;
        public string TransactionId { get; set; } = null!;

        //public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public PaymentStatus Status { get; set; } 
        public DateTime PaidAt { get; set; }
        public decimal Price { get; set; }

        #region RelationShips
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = null!;
        #endregion
    }

}
