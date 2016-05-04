using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperScissorStoneCore;

namespace PaperScissorStone1.Tests.Core
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void Game_CanAddTwoThrows()
        {
            // Arrange
            IGame game = new Game();


            // Act
            try
            {
                game.AddThrow(1, "Paper");
                game.AddThrow(2, "Stone");
            }
            // Assert
            catch
            {
                Assert.Fail("unable to add two throws");
            }            
        }

        [TestMethod]
        public void Game_BreaksOnThreeThrows()
        {
            // Arrange
            IGame game = new Game();


            // Act
            try
            {
                game.AddThrow(1, "Paper");
                game.AddThrow(2, "Stone");
                game.AddThrow(3, "Scissor");

                // Assert
                Assert.Fail("expecting exception with more than two throws");
            }
            catch
            {
            }
        }

        [TestMethod]
        public void Game_PaperBeatsStone()
        {
            // Arrange
            IGame game = new Game();


            // Act
            game.AddThrow(1, "Paper");
            game.AddThrow(2, "Stone");

            // Assert
            Assert.IsTrue(game.Winner.Value == 1, "expecting paper to beat stone");
        }

        [TestMethod]
        public void Game_ScissorBeatsPaper()
        {
            // Arrange
            IGame game = new Game();


            // Act
            game.AddThrow(1, "Paper");
            game.AddThrow(2, "Scissor");

            // Assert
            Assert.IsTrue(game.Winner.Value == 2, "expecting scissor to beat paper");
        }

        [TestMethod]
        public void Game_StoneBeatsScissor()
        {
            // Arrange
            IGame game = new Game();


            // Act
            game.AddThrow(1, "Stone");
            game.AddThrow(2, "Scissor");

            // Assert
            Assert.IsTrue(game.Winner.Value == 1, "expecting stone to beat scissor");
        }


        [TestMethod]
        public void Game_NextRoundWorks()
        {
            // Arrange
            IGame game = new Game();


            // Act
            game.AddThrow(1, "Stone");
            game.AddThrow(2, "Scissor");
            int firstWinner = game.Winner.Value;

            game.NextRound();

            game.AddThrow(1, "Paper");
            game.AddThrow(2, "Scissor");
            int secondWinner = game.Winner.Value;

            // Assert
            Assert.IsTrue(firstWinner == 1 && secondWinner == 2, "two rounds in a row are not working");
        }

        [TestMethod]
        public void Game_StoneDrawsWork()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(1, "Stone");
            game.AddThrow(2, "Stone");

            // Assert
            bool noWinner = (game.Winner.Value == 0);
            bool notFaulted = (game.IsTurnFaulted == false);
            Assert.IsTrue(noWinner && notFaulted, "expecting a draw");
        }

        [TestMethod]
        public void Game_PaperDrawsWork()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(1, "Paper");
            game.AddThrow(2, "Paper");

            // Assert
            bool noWinner = (game.Winner.Value == 0);
            bool notFaulted = (game.IsTurnFaulted == false);
            Assert.IsTrue(noWinner && notFaulted, "expecting a draw");
        }


        [TestMethod]
        public void Game_ScissorsDrawsWork()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(1, "Scissor");
            game.AddThrow(2, "Scissor");

            // Assert
            bool noWinner = (game.Winner.Value == 0);
            bool notFaulted = (game.IsTurnFaulted == false);
            Assert.IsTrue(noWinner && notFaulted, "expecting a draw");
        }


        [TestMethod]
        public void Game_BadSubmissionFaultWorks()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(1, "");
            game.AddThrow(2, "Scissor");

            // Assert
            Assert.IsTrue(game.IsTurnFaulted, "expecting a fault");
        }

        [TestMethod]
        public void Game_NotEnoughSubmissionsFaultWorks()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(2, "Scissor");

            // Assert
            //bool noWinner = (game.Winner.Value == 0);
            Assert.IsTrue(game.IsTurnFaulted, "expecting a fault");
        }

        [TestMethod]
        public void Game_TurnCount()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(2, "Scissor");
            game.NextRound();
            game.AddThrow(1, "Scissor");
            game.AddThrow(2, "Scissor");
            game.NextRound();
            game.AddThrow(1, "Scissor");

            // Assert
            Assert.IsTrue(game.TurnCount == 3, "expecting a different turn count");
        }

        [TestMethod]
        public void Game_MostUsedMove()
        {
            // Arrange
            IGame game = new Game();

            // Act
            game.AddThrow(2, "Scissor");
            game.NextRound();
            game.AddThrow(1, "Scissor");
            game.AddThrow(2, "Scissor");
            game.NextRound();
            game.AddThrow(1, "Scissor");
            game.NextRound();
            game.AddThrow(2, "Stone");
            game.NextRound();
            game.AddThrow(1, "Stone");
            game.AddThrow(2, "Paper");
            game.NextRound();
            game.AddThrow(1, "Paper");


            // Assert
            Assert.IsTrue(game.MostOccuringMove == ThrowType.Scissor, "expecting Scissor");
        }
    }
}
