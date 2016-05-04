using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;

namespace PaperScissorStoneCore
{
    public interface IPlayerManager
    {
        int? LogOn(string name, string password);
        void LogOff(int id);
        /// <summary>
        /// Logs off if no further activity within a specified time
        /// </summary>
        /// <param name="id">Id of player to log off if idle</param>
        /// <param name="secondsWait">Seconds to wait for activity before logging off</param>
        void PotentiallyLoggOff(int id, int secondsWait);
        ///// <summary>
        ///// Event triggered if a player is logged off because of inactivity
        ///// </summary>
        //event EventHandler<InactivityLoggOffArgs> InactiveLogOff;
        int? Register(string name, string password);
        bool IsDuplicateName(string name);
        void UpdateActivity(int id);
        List<IPlayer> LoggedOn { get; }
    }
       
    [Export(typeof(IPlayerManager))]
    public class PlayerManager : IPlayerManager, IDisposable
    {
        private static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;AttachDbFilename=|DataDirectory|\PSS001.mdf";

        //private static IPlayerManager _single = null;
        //public static IPlayerManager Single
        //{
        //    get
        //    {
        //        if (_single == null)
        //        {
        //            _single = new PlayerManager();
        //        }
        //        return _single;
        //    }
        //}

        private SqlConnection Connection { get; set; }

        private object PlayerLock = new object();
        /// <summary>
        /// Guarded with PlayerLock
        /// </summary>
        private List<Player> Players { get; set; }
        /// <summary>
        /// Guarded with PlayerLock
        /// </summary>
        private List<int> DuplicateNameCheck { get; set; }
        /// <summary>
        /// Guarded with PlayerLock
        /// </summary>
        private Dictionary<int, Timer> LoggingOff { get; set; }

        public List<IPlayer> LoggedOn { get { lock (PlayerLock) return Players.Cast<IPlayer>().ToList(); } }

        public PlayerManager()
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();

            lock (PlayerLock)
            {
                Players = new List<Player>();
                DuplicateNameCheck = GetPlayers()
                    .Select(s => s.Name.GetHashCode())
                    .ToList();

                LoggingOff = new Dictionary<int, Timer>();
            }
        }

        public void Dispose()
        {
            Connection.Close();
        }

        private IList<Player> GetPlayers()
        {
            List<Player> retval = new List<Player>();

            using (var command = new SqlCommand("SELECT Id, Name FROM dbo.Players", Connection))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                    return retval;

                while (reader.Read())
                {
                    var p = new Player(
                        id: reader.GetInt32(0),
                        name: reader.GetString(1));

                    retval.Add(p);
                }
            }
            return retval;
        }

        public int? Register(string name, string password)
        {
            const string sql = @"INSERT INTO Players(Name, Password) 
OUTPUT INSERTED.Id
VALUES(@name, @password)";

            int? newId = null;
            using (var command = new SqlCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@password", password);

                newId = command.ExecuteScalar() as int?;
            }


            if (newId.HasValue)
                return LogOn(name, password);

            return newId;            
        }

        public bool IsDuplicateName(string name)
        {
            lock (PlayerLock)
                return DuplicateNameCheck.Any(a => a == name.GetHashCode());
        }

        private int? CheckPassword(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                return null;

            int? retval = null;
            const string sql = @"SELECT TOP 1 Id FROM dbo.Players 
WHERE @name = Name AND @password = Password";

            using (var command = new SqlCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@password", password);

                object result = command.ExecuteScalar();
                retval = result as int?;
            }

            return retval;
        }

        private static Player GetPlayer(List<Player> list, int id)
        {
            var found = list.Find(f => f.Id == id);
            return found;
        }

        private static void CleanTimer(Timer cleanMe)
        {
            cleanMe.Stop();
            cleanMe.Dispose();
        }

        public void UpdateActivity(int id)
        {
            lock (PlayerLock)
            {
                if (LoggingOff.ContainsKey(id))
                {
                    CleanTimer(LoggingOff[id]);
                    LoggingOff.Remove(id);
                }

                var found = GetPlayer(Players, id);
                if (found != null)
                    found.LastActivityOn = DateTime.Now;
            }
        }

        public void LogOff(int id)
        {
            lock (PlayerLock)
            {
                var found = GetPlayer(Players, id);
                if (found != null)
                    Players.Remove(found);
            }
        }

        public int? LogOn(string name, string password)
        {
            int? id = CheckPassword(name, password);
            if (id.HasValue)
            {
                lock (PlayerLock)
                {
                    Players.Add(new Player(id.Value, name));
                    DuplicateNameCheck.Add(name.GetHashCode());
                }

                UpdateActivity(id.Value);
            }
            return id;
        }

        public void PotentiallyLoggOff(int id, int secondsWait)
        {
            lock (PlayerLock)
            {
                // if the id not logged on, ignore and return
                if (!Players.Any(a => a.Id == id))
                    return;

                // if already running an inactivity countdown then no need to progress, return
                if (LoggingOff.ContainsKey(id) && LoggingOff[id].Enabled)
                    return;

                var timer = new Timer()
                    { Interval = secondsWait * 1000, AutoReset = false, Enabled = false };

                timer.Elapsed += (object sender, ElapsedEventArgs e) => Timer_Elapsed((Timer)sender, id);
                LoggingOff[id] = timer;
                LoggingOff[id].Start();
            }
        }

        private void Timer_Elapsed(Timer sender, int id)
        {
            var me = (Timer)sender;

            LogOff(id);
            CleanTimer(LoggingOff[id]);

            lock (PlayerLock)
                LoggingOff.Remove(id);
        }
    }
}
