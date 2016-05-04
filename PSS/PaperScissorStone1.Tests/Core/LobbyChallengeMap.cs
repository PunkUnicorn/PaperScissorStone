using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperScissorStoneCore;
using System.Collections.Generic;

namespace PaperScissorStone1.Tests.Core
{
    [TestClass]
    public class LobbyChallengeMapTests
    {
        [TestMethod]
        public void LobbyChallengeMap_GetFirst()
        {
            // Arrange
            const int challenger_id = 1;
            const int other_id = 2;
            const int ignore1 = 3;
            const int ignore2 = 3;
            const int ignore3 = 4;
            const int ignore4 = 5;
            var m = new LobbyChallengeMap();
            m.Set(challenger_id, other_id);
            m.Set(ignore1, ignore2);
            m.Set(ignore3, ignore4);

            // Act
            IEnumerable<IChallengePair> actual = m.Get(challenger_id);

            // Assert
            var listCheck = new List<IChallengePair>(actual);
            Assert.IsTrue(listCheck.Count == 1, "Expecting one result");
            Assert.IsTrue(listCheck[0].Challenger == challenger_id, "Unexpected return result");
        }

        [TestMethod]
        public void LobbyChallengeMap_GetSecond()
        {
            // Arrange
            const int challenger_id = 1;
            const int other_id = 2;
            const int ignore1 = 3;
            const int ignore2 = 3;
            const int ignore3 = 4;
            const int ignore4 = 5;
            var m = new LobbyChallengeMap();
            m.Set(challenger_id, other_id);
            m.Set(ignore1, ignore2);
            m.Set(ignore3, ignore4);

            // Act
            IEnumerable<IChallengePair> actual = m.Get(other_id);

            // Assert
            var listCheck = new List<IChallengePair>(actual);
            Assert.IsTrue(listCheck.Count == 1, "Expecting one result");
            Assert.IsTrue(listCheck[0].Challengee == other_id, "Unexpected return result");
        }

        [TestMethod]
        public void LobbyChallengeMap_CorrectlyNotGet()
        {
            // Arrange
            const int challenger_id = 1;
            const int other_id = 2;
            const int ignore1 = 3;
            const int ignore2 = 3;
            const int ignore3 = 4;
            const int ignore4 = 5;
            const int not_present = 6;
            var m = new LobbyChallengeMap();
            m.Set(challenger_id, other_id);
            m.Set(ignore1, ignore2);
            m.Set(ignore3, ignore4);

            // Act
            IEnumerable<IChallengePair> actual = m.Get(not_present);

            // Assert
            var listCheck = new List<IChallengePair>(actual);
            Assert.IsTrue(listCheck.Count == 0, "Not expecting a return result");
        }

        [TestMethod]
        public void LobbyChallengeMap_CanRemove()
        {
            // Arrange
            const int remove_id1 = 1;
            const int remove_id2 = 2;
            var m = new LobbyChallengeMap();
            m.Set(remove_id1, remove_id2);

            // Act
            m.Remove(remove_id1, remove_id2);

            // Assert
            var listCheck = new List<IChallengePair>( m.Get(remove_id1) );
            Assert.IsFalse(listCheck.Count != 0, "Item not removed");
        }

        [TestMethod]
        public void LobbyChallengeMap_CanRemoveSwapped()
        {
            // Arrange
            const int remove_id1 = 1;
            const int remove_id2 = 2;
            var m = new LobbyChallengeMap();
            m.Set(remove_id1, remove_id2);

            // Act
            m.Remove(remove_id2, remove_id1); //swap parameter order

            // Assert
            var listCheck = new List<IChallengePair>(m.Get(remove_id1));
            Assert.IsFalse(listCheck.Count != 0, "Item not removed");
        }
    }
}
