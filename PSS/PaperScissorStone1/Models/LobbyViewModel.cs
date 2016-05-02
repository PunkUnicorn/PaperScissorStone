using PaperScissorStoneCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperScissorStone1.Models
{
    public class LobbyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}