using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Wishlist : BaseEntity<int>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();


        public int UserId { get; set; }
        public User User { get; set; } = null!;

        
    }
}
