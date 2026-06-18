using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Address : BaseEntity<int>
    {

        public string Country { get; set; } = null!;

        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string BuildingNo { get; set; } = null!;

        #region RelationShips
        public int UserId { get; set; }
        public User User { get; set; } = null!; 
        #endregion
    }

}
