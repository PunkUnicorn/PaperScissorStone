using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PaperScissorStoneCore
{
    public interface ILobbyConnectionMap
    {
        string Get(int id);
        void Set(int id, string connectionId);
    }

    public interface ILobbyChallengeMap
    {
        IEnumerable<IChallengePair> Get(int id);
        void Set(int challenger, int challengee);
        void Remove(int challenger, int challengee);
    }

    [Export(typeof(ILobbyConnectionMap))]
    public class LobbyConnectionMap : ILobbyConnectionMap
    {
        //private static readonly Lazy<LobbyConnectionMap> _lobbyConnectionMap = new Lazy<LobbyConnectionMap>(()=>new LobbyConnectionMap());
        //public static ILobbyConnectionMap Single { get { return _lobbyConnectionMap.Value; } }

        private readonly Dictionary<int /*player Id*/, string /*signalR ConnectionId*/> PlayerConnectionId = new Dictionary<int, string>();
        private readonly object Lock = new object();

        public string Get(int id) { lock (Lock) return PlayerConnectionId[id]; }
        public void Set(int id, string connectionId) { lock (Lock) PlayerConnectionId[id] = connectionId; }
    }

    public interface IChallengePair
    {
        int Challenger { get; }
        int Challengee { get; }
        int Max { get; }
        int Min { get; }
        /// <summary>
        /// Returns true if this Id is involved in this challenge
        /// </summary>
        /// <param name="id">Id to test</param>
        /// <returns>true if Id is involved in this challenge</returns>
        bool Contains(int id);
    }

    public class ChallengePair : IEquatable<IChallengePair>, IChallengePair
    {
        public ChallengePair(int challenger, int challengee)
        {
            Challenger = challenger;
            Challengee = challengee;
            Max = Math.Max(challenger, challengee); //<- Max and Min called frequently so pre-calc
            Min = Math.Min(challenger, challengee);
        }
        public int Challenger { get; private set; }
        public int Challengee { get; private set; }
        public int Max { get; private set; }
        public int Min { get; private set; }
        public bool Equals(IChallengePair other) { return Max == other.Max && Min == other.Min; }
        public override int GetHashCode() { return Min.GetHashCode() * 17 + Max.GetHashCode(); }
        public bool Contains(int id) { return Max == id || Min == id; }
    }

    [Export(typeof(ILobbyChallengeMap))]
    public class LobbyChallengeMap : ILobbyChallengeMap
    {
        private object Lock = new object();
        private HashSet<IChallengePair> Challenges = new HashSet<IChallengePair>();

        public IEnumerable<IChallengePair> Get(int id)
        {
            lock (Lock)
                return Challenges
                    .Where(a => a.Contains(id))
                    .ToList();
        }

        public void Remove(int challenger, int challengee)
        {
            var pair = new ChallengePair(challenger, challengee);
            lock (Lock)
            {
                if (Challenges.Contains(pair))
                {
                    Challenges.Remove(pair);
                }
            }
        }

        public void Set(int challenger, int challengee)
        {
            lock (Lock)
                Challenges.Add(new ChallengePair(challenger, challengee));
        }
    }
}
