using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class UserPhone : BaseEntity<int>
    {
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        #region RelationShips
        public int UserId { get; set; }
        public User User { get; set; } = null!; 
        #endregion


    }
}
