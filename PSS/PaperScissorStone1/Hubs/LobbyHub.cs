using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PaperScissorStoneCore;

namespace PaperScissorStone1
{
    // signalR with MVC 5 Tutorial: http://www.asp.net/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc
    public class LobbyHub : Hub
    {
        public IPlayerManager DataContext { get { return PlayerManager.Single; } }
        public ILobbyConnectionMap ConnectionMap { get { return LobbyConnectionMap.Single; } }

        public void AddPlayer(int id, string name)
        {
            ConnectionMap.Set(id, Context.ConnectionId);
            DataContext.UpdateActivity(id);
            Clients.All.addPlayer(id, name);
        }

        //public static void InactiveLogOff(object sender, InactivityLoggOffArgs e)
        //{
        //    // http://stackoverflow.com/questions/17395201/call-a-hub-method-from-a-controllers-action/17396729#17396729
        //    var hubContext = GlobalHost.ConnectionManager.GetHubContext<LobbyHub>();
        //    hubContext.Clients.All.removePlayer(e.Id);
        //}

        public void RemovePlayer(int id)
        {
            DataContext.PotentiallyLoggOff(id, 5);
            Clients.All.removePlayer(id);
        }

        public void Send(string name, string message)
        {
            Clients.All.newMessage(name, message);
        }

        public void StartChallenge(int myId, string myName, int theirId)
        {
            var connectionId = ConnectionMap.Get(theirId);
            Clients.Client(connectionId).startChallenge(myId, myName);
        }
    }
}