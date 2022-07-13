using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserInChat
    {
        [Key]
        public Guid IdUserInChat { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid IdUser { get; set; }
        [Required]
        [ForeignKey("Chat")]
        public Guid IdChat { get; set; }
        [Required, DefaultValue(0)]
        public int NewMessagesInChat { get; set; } = 0;
        [Required, DefaultValue(false)]
        public bool IsMuted { get; set; } = false;
    }
}
