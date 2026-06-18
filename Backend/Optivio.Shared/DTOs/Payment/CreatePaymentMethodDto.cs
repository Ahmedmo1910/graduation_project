using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Shared.DTOs.Payment
{
    public class CreatePaymentMethodDto
    {
        public string Provider { get; set; } = null!;
        public string LastDigits { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
        public bool IsDefault { get; set; }
    }
}
