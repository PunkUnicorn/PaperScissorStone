using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperScissorStoneCore
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastActivityOn { get; set; }
    }

    public class Game
    {
        public int Id { get; set; }
        public int LeftPlayerId { get; set; }
        public int RightPlayerId { get; set; }
    }

    public class Turn
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int? WinnerId { get; set; }
    }
}
