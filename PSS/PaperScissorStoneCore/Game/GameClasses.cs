using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperScissorStoneCore
{
    public interface IPlayer
    {
        int Id { get; }
        string Name { get; }
        DateTime LastActivityOn { get; }
    }

    public class Player : IPlayer
    {
        public Player(int id, string name) { Id = id;  Name = name; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime LastActivityOn { get; internal set; }
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
