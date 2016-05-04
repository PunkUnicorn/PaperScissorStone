using System;
using System.Collections.Generic;

namespace PaperScissorStoneCore
{
    public interface IChallengePair : IEquatable<IChallengePair>
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

    public class ChallengePair : IChallengePair
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
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = hash * 31 + Min.GetHashCode();
                hash = hash * 31 + Max.GetHashCode();
                return hash;
            }
        }
        public bool Contains(int id) { return Max == id || Min == id; }
    }
}
