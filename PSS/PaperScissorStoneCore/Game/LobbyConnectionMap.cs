using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperScissorStoneCore
{
    public interface ILobbyConnectionMap
    {
        string Get(int id);
        void Set(int id, string connectionId);
    }

    public class LobbyConnectionMap : ILobbyConnectionMap
    {
        private static readonly Lazy<LobbyConnectionMap> _lobbyConnectionMap = new Lazy<LobbyConnectionMap>(()=>new LobbyConnectionMap());
        public static ILobbyConnectionMap Single { get { return _lobbyConnectionMap.Value; } }

        private readonly Dictionary<int /*player Id*/, string /*signalR ConnectionId*/> PlayerConnectionId = new Dictionary<int, string>();
        private object Lock = new object();

        public string Get(int id) { lock (Lock) return PlayerConnectionId[id]; }
        public void Set(int id, string connectionId) { lock (Lock) PlayerConnectionId[id] = connectionId; }
    }
}
