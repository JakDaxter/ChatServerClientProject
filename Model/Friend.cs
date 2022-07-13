using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Friend
    {
        [Key]
        public Guid IdFriends { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid IdUser1 { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid IdUser2 { get; set; }
        [Required]
        public DateTime DateTimeWhenRelacionWasCreated { get; set; }
    }
}
