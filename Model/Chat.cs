using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Chat
    {
        [Key]
        public Guid IdChat { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsCreateDefault { get; set; }
        [Required]
        public DateTime LastMessageDate {get;set;}
    }
}
