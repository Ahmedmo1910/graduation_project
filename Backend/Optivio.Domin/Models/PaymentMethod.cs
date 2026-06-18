using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class PaymentMethod : BaseEntity<int>
    {
        public string Provider { get; set; } = null!;
        public string LastDigits { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
        public bool IsDefault { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
