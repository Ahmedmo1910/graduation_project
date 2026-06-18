using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Optivio.Domin.Models
{
    public class User : BaseEntity<int>
    {

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public ICollection<UserPhone> Phones { get; set; }= new List<UserPhone>();
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }


        public UserRole Role { get; set; }

        public Status Status { get; set; }


        public Profile Profile { get; set; } = null!;
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        #region RelationShips
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();

        public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

        #endregion
    }

}
