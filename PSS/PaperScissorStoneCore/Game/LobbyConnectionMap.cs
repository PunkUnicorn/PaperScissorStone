using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PaperScissorStoneCore
{
    public interface ISignalRConnectionMap
    {
        string Get(int id);
        void Set(int id, string connectionId);
    }

    [Export(typeof(ISignalRConnectionMap))]
    public class SignalRConnectionMap : ISignalRConnectionMap
    {
        private readonly Dictionary<int /*player Id*/, string /*signalR ConnectionId*/> PlayerConnectionId = new Dictionary<int, string>();
        private readonly object Lock = new object();

        public string Get(int id) { lock (Lock) return PlayerConnectionId.ContainsKey(id) ? PlayerConnectionId[id] : string.Empty; }
        public void Set(int id, string connectionId) { lock (Lock) PlayerConnectionId[id] = connectionId; }
    }
}
