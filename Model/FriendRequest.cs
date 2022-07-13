using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FriendRequest
    {
        [Key]
        public Guid IdFriendRequest { get; set; }
        [Required]
        public DateTime DateTimeWhenRequestWasCreated { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid IdFromUser { get; set; }
        [Required, ForeignKey("User")]
        public Guid IdToUser { get; set; }
    }

}
