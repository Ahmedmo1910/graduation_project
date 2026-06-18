using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class WishlistItem : BaseEntity<int>
    {
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
