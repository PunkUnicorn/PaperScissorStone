using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperScissorStoneCore
{
    public interface ILobbyChallengeMap
    {
        /// <summary>
        /// Gets the challenge entries that contain id as either the challenger or the challengee
        /// </summary>
        /// <param name="id">Id to retrieve all challenges for</param>
        /// <returns></returns>
        IEnumerable<IChallengePair> Get(int id);
        void Set(int challenger, int challengee);
        void Remove(int challenger, int challengee);
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
