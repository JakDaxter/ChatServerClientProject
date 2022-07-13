using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ItemInTable
    {
        public ItemInTable() { }
        public string IdChat { get; set; }
        public string Name { get; set; }
        public bool IsCreatedDefaul { get; set; }
        public int NewMessagesInChat {get;set;}
        public bool IsMuted { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}
