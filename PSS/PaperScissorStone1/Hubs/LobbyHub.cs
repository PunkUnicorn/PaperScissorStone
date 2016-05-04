using Microsoft.AspNet.SignalR;
using PaperScissorStoneCore;

namespace PaperScissorStone1
{
    // signalR with MVC 5 Tutorial: http://www.asp.net/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc
    public class LobbyHub : Hub
    {
        private IPlayerManager DataContext { get; set; }
        private ILobbyConnectionMap ConnectionMap { get; set; }
        private ILobbyChallengeMap ChallengeMap { get; set; }
        
        public LobbyHub(IPlayerManager dataContext, ILobbyConnectionMap connectionMap, ILobbyChallengeMap challengeMap)
        {
            ChallengeMap = challengeMap;
            ConnectionMap = connectionMap;
            DataContext = dataContext;
        }

        public void AddPlayer(int id, string name)
        {
            ConnectionMap.Set(id, Context.ConnectionId);
            DataContext.UpdateActivity(id);
            Clients.All.addPlayer(id, name);
        }

        /// <summary>
        /// Removes a player from the lobby list, retracts their own challenges and rejects others. This is for cases when the window is closed.
        /// However to accomodate F5 refresh the player isn't logged off untill a short period of inactivity.
        /// </summary>
        /// <param name="id">Id of player to remove</param>
        public void RemovePlayer(int id)
        {
            DataContext.PotentiallyLoggOff(id, 15);

            foreach (var involvement in ChallengeMap.Get(id))
            {
                bool isChallenger = (involvement.Challenger == id);

                if (isChallenger)
                {
                    // retract our challenges
                    var removeConId = ConnectionMap.Get(involvement.Challengee);
                    Clients.Client(removeConId).challengeCancelled(involvement.Challenger);
                }
                else
                {
                    // reject those of others
                    var removeConId = ConnectionMap.Get(involvement.Challenger);
                    Clients.Client(removeConId).challengeRejected(involvement.Challengee);
                }
            }
            Clients.All.removePlayer(id);
        }

        public void Send(string name, string message)
        {
            Clients.All.newMessage(name, message);
        }

        public void InviteChallenge(int myId, string myName, int theirId)
        {
            var connectionId = ConnectionMap.Get(theirId);
            ChallengeMap.Set(myId, theirId);
            Clients.Client(connectionId).inviteChallenge(myId, myName);
        }

        /// <summary>
        /// Allows the caller of the challenge to cancel
        /// </summary>
        /// <param name="myId">Challenger id</param>
        /// <param name="myName">Challenger name</param>
        /// <param name="theirId">User being challenged</param>
        public void CancelChallenge(int myId, string myName, int theirId)
        {
            ChallengeMap.Remove(myId, theirId);
            var connectionId = ConnectionMap.Get(theirId);
            Clients.Client(connectionId).challengeCancelled(myId);
        }

        /// <summary>
        /// Allows the victum of a challenge to reject back to the user who called it
        /// </summary>
        /// <param name="myId">Id of challenge rejecter</param>
        /// <param name="theirId">Id of challenge caller</param>
        public void RejectChallenge(int myId, int theirId)
        {
            ChallengeMap.Remove(theirId, myId);
            var connectionId = ConnectionMap.Get(theirId);
            Clients.Client(connectionId).challengeRejected(myId);
        }
    }
}