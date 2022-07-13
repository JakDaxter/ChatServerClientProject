

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Index(nameof(IdChat), nameof(ExpedationDate))]
    public class Message
    {
        [Key]
        public Guid IdMessage { get; set; }
        [Required]
        public DateTime ExpedationDate { get; set; }
        [Required]
        [StringLength(280)]
        public string Content { get; set; }
        [Required]
        [ForeignKey("Chat")]
        public Guid IdChat { get; set; }
        [Required]
        public string NameOfShipper { get; set; }
    }
}
