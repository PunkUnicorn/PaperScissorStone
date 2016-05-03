using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PaperScissorStoneCore
{
    public interface IPlayerManager
    {
        int? LogOn(string name, string password);
        void LogOff(int id);
        /// <summary>
        /// Logs off if no further activity for a short time
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
        List<Player> LoggedOn { get; }
    }
       
    [Export(typeof(IPlayerManager))]
    public class PlayerManager : IPlayerManager, IDisposable
    {
        private static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;AttachDbFilename=|DataDirectory|\PSS001.mdf";

        private static IPlayerManager _single = null;
        public static IPlayerManager Single
        {
            get
            {
                if (_single == null)
                {
                    _single = new PlayerManager();
                }
                return _single;
            }
        }

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

        public List<Player> LoggedOn { get { lock (PlayerLock) return Players.ToList(); } }

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
                    retval.Add(new Player
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }

            return retval;
        }

        public int? Register(string name, string password)
        {
            const string sql = @"INSERT INTO Players(Name, Password) 
OUTPUT INSERTED.Id
VALUES(@name, @password)";

            //DateTime activityTime = DateTime.Now;
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
            //if (found == null)
            //    throw new InvalidOperationException("GetPlayer failed for unknown player id:" + id);

            return found;
        }

        public void UpdateActivity(int id)
        {
            lock (PlayerLock)
            {
                if (LoggingOff.ContainsKey(id))
                    LoggingOff[id].Stop();

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
                    Players.Add(new Player { Name = name, Id = id.Value });
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

                timer.Elapsed += (object sender, ElapsedEventArgs e) => LogOff(id);
                LoggingOff[id] = timer;
                LoggingOff[id].Start();
            }
        }
    }
}
