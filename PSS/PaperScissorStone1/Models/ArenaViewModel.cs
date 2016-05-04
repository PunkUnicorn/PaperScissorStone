using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperScissorStone1.Models
{
    public class ArenaViewModel
    {
        public int GameId { get; set; }
        public int MyId { get; set; }
        public int TheirId { get; set; }
        public int TurnId { get; set; }
        public int TurnCount { get; set; }
        public string MostUsedMove { get; set; }
    }
}