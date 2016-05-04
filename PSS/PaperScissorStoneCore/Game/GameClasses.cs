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
        int ThrowCount { get; }
        int ?Winner { get; }

        void AddThrow(int id, string throwType);
        bool IsTurnFaulted { get; }

        void NextRound();
    }

    public class Game : IGame
    {
        public Game()
        {
            Turns = new List<Turn>();
        }

        private object Lock = new object();
        public int Id { get; set; }
        public int LeftPlayerId { get; set; }
        public int RightPlayerId { get; set; }
        /// <summary>
        /// Guarded with Lock
        /// </summary>
        private Turn CurrentTurn { get; set; }
        /// <summary>
        /// Guarded with Lock
        /// </summary>
        private List<Turn> Turns { get; set; }
        public int ThrowCount
        {
            get
            {
                lock (Lock)
                {
                    if (CurrentTurn == null)
                        return 0;

                    return CurrentTurn.Throws.Count;
                }
            }
        }

        public bool IsTurnFaulted
        {
            get
            {
                // This really should call down to the Turn class

                lock (Lock)
                    if (CurrentTurn == null) return true;

                lock (Lock)
                {
                    var isInvalidThrow = CurrentTurn.Throws.Count != 2 || 
                            CurrentTurn.Throws.Values.Any(a => a == ThrowType.Invalid);

                    return isInvalidThrow;
                }
            }

        }

        public void AddThrow(int id, string submittedThrow)
        {
            lock (Lock)
            {
                Turn turn = CurrentTurn;
                if (CurrentTurn == null)
                {
                    turn = new Turn();
                    Turns.Add(turn);
                    CurrentTurn = turn;
                }

                ThrowType throwType;
                try
                {
                    throwType = (ThrowType)Enum.Parse(typeof(ThrowType), submittedThrow);
                }
                catch
                {
                    throwType = ThrowType.Invalid;
                }

                turn.Throws.Add(id, throwType);
            }
        }

        public int? Winner
        {
            get
            {
                lock (Lock)
                {
                    if (CurrentTurn == null)
                        return null;

                    return CurrentTurn.WinnerId;
                }
            }
        }

        public void NextRound()
        {
            lock (Lock)
                CurrentTurn = null;
        }
    }

    public enum ThrowType
    {
        Invalid,
        Paper,
        Scissor,
        Stone
    }

    public class ThrowComparor : IComparer<ThrowType>
    {
        public int Compare(ThrowType x, ThrowType y)
        {
            if (x == y) return 0;
            if (x == ThrowType.Invalid) return 1;
            if (y == ThrowType.Invalid) return -1;
            if (x == ThrowType.Paper && y == ThrowType.Scissor) return 1;
            if (x == ThrowType.Paper && y == ThrowType.Stone) return -1;
            if (x == ThrowType.Scissor && y == ThrowType.Paper) return -1;
            if (x == ThrowType.Scissor && y == ThrowType.Stone) return 1;
            if (x == ThrowType.Stone && y == ThrowType.Paper) return 1;
            if (x == ThrowType.Stone && y == ThrowType.Scissor) return -1;
            return 0;
        }
    }

    public class Turn
    {
        public Turn()
        {
            Throws = new Dictionary<int /*playerId*/, ThrowType>();
        }

        public Dictionary<int, ThrowType> Throws { get;set; }
        public int? WinnerId
        {
            get
            {
                if (Throws.Keys.Count != 2)
                    return null;

                var key1 = Throws.Keys.First();
                var key2 = Throws.Keys.Last();
                var tester = new ThrowComparor();
                int result = tester.Compare(Throws[key1], Throws[key2]);
                
                if (result == 0)
                    return 0;

                if (result < 0)
                    return key1;

                return key2;
            }
        }
    }
}
