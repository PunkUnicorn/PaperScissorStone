using PaperScissorStoneCore;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System;

namespace PaperScissorStone1
{
    public interface IArenaManager
    {
        IGame NewGame(int challenger, int accepter);
        IGame Get(int gameId);
        void NextRound(int gameId);
    }

    [Export(typeof(IArenaManager))]
    public class ArenaManager : IArenaManager
    {
        private readonly object Lock = new object();
        private List<IGame> Games { get; set; }

        public ArenaManager()
        {
            Games = new List<IGame>();
        }

        public IGame NewGame(int challenger, int accepter)
        {
            IGame retval = null;
            lock (Lock)
            {
                int id = Games.Count + 1;
                retval = new Game() { Id = id, LeftPlayerId = challenger, RightPlayerId = accepter };
                Games.Add(retval);
            }

            return retval;
        }

        public IGame Get(int gameId)
        {
            lock (Lock)
                return Games.FirstOrDefault(f => f.Id == gameId);
        }

        public void NextRound(int gameId)
        {
            lock (Lock)
            {
                var game = Games.FirstOrDefault(f => f.Id == gameId);
                game.NextRound();
            }
        }
    }
}