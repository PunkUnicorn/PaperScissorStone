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

    public interface IGame
    {
        int Id { get; }
        int LeftPlayerId { get; }
        int RightPlayerId { get; }
    }

    public class Game : IGame
    {
        public int Id { get; set; }
        public int LeftPlayerId { get; set; }
        public int RightPlayerId { get; set; }
        public int JoinCount { get; set; }
        public List<Turn> Turns { get; set; }
        public int? Winner
        {
            get;
        }
    }

    public class Turn
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int? WinnerId { get; set; }
    }
}
