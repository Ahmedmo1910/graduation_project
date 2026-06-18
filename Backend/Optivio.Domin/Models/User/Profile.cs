using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Profile : BaseEntity<int>
    {
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        #region RelationShips
        public int UserId { get; set; }
        public User User { get; set; } = null!; 
        #endregion
    }

}
