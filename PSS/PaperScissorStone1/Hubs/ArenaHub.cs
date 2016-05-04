using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PaperScissorStoneCore;
using System.Timers;

namespace PaperScissorStone1
{
    public class ArenaHub : Hub
    {
        private IPlayerManager DataContext { get; set; }
        private ISignalRConnectionMap ConnectionMap { get; set; }
        private IArenaManager ArenaManager { get; set; }

        public ArenaHub(IPlayerManager dataContext, ISignalRConnectionMap connectionMap, IArenaManager arenaManager)
        {
            ConnectionMap = connectionMap;
            DataContext = dataContext;
            ArenaManager = arenaManager;
        }

        public void LeftArena(int id)
        {

        }

        private class CountdownTimer : Timer
        {
            public int GameId { get; set; }
            public int Countdown { get; set; }
            public IEnumerable<string> Connections { get; set; }
        }

        private void StartCountdown(IEnumerable<string> con, int gameId)
        {
            var t = new CountdownTimer()
                { Interval = 1000, AutoReset = true, Enabled = false };

            t.Countdown = 3;
            t.GameId = gameId;
            t.Connections = con;

            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private static void CleanTimer(Timer me)
        {
            me.Stop();
            me.Dispose();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            var me = (CountdownTimer)sender;
            var game = ArenaManager.Get(me.GameId);
            if (game == null)
            {
                CleanTimer(me);
                return;
            }

            if (me.Countdown > 0)
            {
                Clients.Clients(me.Connections.ToList()).countdown(me.Countdown);
                me.Countdown -= 1;
            }
            else
            {
                Clients.Clients(me.Connections.ToList()).pleaseSubmitThrow();
                CleanTimer(me);
            }
        }

        public void LeftArena(int gameId, int id)
        {
            DataContext.PotentiallyLoggOff(id, 15);
        }

        public void Joined(int gameId, int id)
        {
            ConnectionMap.Set(id, Context.ConnectionId);
            DataContext.UpdateActivity(id);
        }

        public void StartRound(int gameId, int id)
        {
            var game = ArenaManager.Get(gameId);
            var con1 = ConnectionMap.Get(game.LeftPlayerId);
            var con2 = ConnectionMap.Get(game.RightPlayerId);

            if (string.IsNullOrWhiteSpace(con1) || string.IsNullOrWhiteSpace(con2))
                return;

            // wait for both then send one, two, three
            StartCountdown(new[] { con1, con2 }, gameId);

            // then on third clients submit their throw
            // within a window collect both throws
            // send winner, update database
            // wait for start round to be pressed, goto 10

        }

        public void SubmitThrow(int id, int throwType)
        {

        }
    }
}