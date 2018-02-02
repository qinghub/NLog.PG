using NLog.Common;
using NLog.Config;
using NLog.Targets;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NLog.PG.NetCore
{

    [Target("PG")]
    public class PGTarget : Target, IPGTarget
    {
        #region Fields

        private static readonly ConcurrentDictionary<string, NpgsqlConnection> _collectionCache = new ConcurrentDictionary<string, NpgsqlConnection>();


        [ArrayParameter(typeof(PGField), "field")]
        public IList<PGField> Fields { get; private set; }

        [ArrayParameter(typeof(PGField), "property")]
        public IList<PGField> Properties { get; private set; }

        public string ConnectionString { get; set; }
        public string TableName { get; set; }


        #endregion


        #region Constructors

        public PGTarget()
        {
            Fields = new List<PGField>();
            Properties = new List<PGField>();
        }

        #endregion


        #region Overrides

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (string.IsNullOrEmpty(ConnectionString))
                throw new NLogConfigurationException("Can not resolve PG ConnectionString. Please make sure the ConnectionString property is set.");
        }


        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            if (logEvents.Length == 0)
                return;

            try
            {
                var con = GetConnection();
                var cmds = logEvents.Select(e => CreateCommand(e.LogEvent));

                if (con.State == ConnectionState.Closed)
                    con.Open();

                foreach (var cmd in cmds)
                {
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                foreach (var ev in logEvents)
                    ev.Continuation(null);

            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException || ex is NLogConfigurationException)
                    throw;

                InternalLogger.Error("Error when writing to PG {0}", ex);

                foreach (var ev in logEvents)
                    ev.Continuation(ex);

            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                var con = GetConnection();
                var cmd = CreateCommand(logEvent);
                cmd.Connection = con;

                if (con.State == ConnectionState.Closed)
                    con.Open();

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                if (ex is OutOfMemoryException || ex is NLogConfigurationException)
                    throw;

                InternalLogger.Error("Error when writing to PG {0}", ex);
            }
        }

        #endregion


        #region Private Methods

        private NpgsqlCommand CreateCommand(LogEventInfo logEvent)
        {
            var cmd = new NpgsqlCommand();

            string fields = string.Empty;
            string values = string.Empty;

            foreach (var field in Fields)
            {
                var value = GetValue(field, logEvent);
                fields += field.Name + ",";
                values += "@" + field.Name + ",";
                if (value != null)
                    cmd.Parameters.AddWithValue(field.Name, value);
                else
                    cmd.Parameters.AddWithValue(field.Name, DBNull.Value);
            }
            cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", TableName, fields.TrimEnd(','),
                values.TrimEnd(','));

            return cmd;
        }

        private object GetValue(PGField field, LogEventInfo logEvent)
        {
            var value = field.Layout.Render(logEvent);
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (string.IsNullOrEmpty(field.PGType)
                || string.Equals(field.PGType, "String", StringComparison.OrdinalIgnoreCase))
                return value;

            if (string.Equals(field.PGType, "Int32", StringComparison.OrdinalIgnoreCase))
                return Convert.ToInt32(value);

            if (string.Equals(field.PGType, "DateTime", StringComparison.OrdinalIgnoreCase))
                return Convert.ToDateTime(value);

            if (string.Equals(field.PGType, "Guid", StringComparison.OrdinalIgnoreCase))
                return new Guid(value);

            return value;
        }

        private NpgsqlConnection GetConnection()
        {
            string key = ConnectionString;

            return _collectionCache.GetOrAdd(key, k =>
            {
                var conn = new NpgsqlConnection(ConnectionString);

                return conn;
            });
        }

        #endregion
    }
}
