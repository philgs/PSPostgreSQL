using System;
using System.Management.Automation;
using Npgsql;

namespace PSPostgreSQL
{
    /// <summary>
    /// Connects to a PostgreSQL database and stores the connection in session state.
    /// </summary>
    /// <remarks>
    /// The connection remains open until Disconnect-PGDatabase is called. Be sure to
    /// always call Disconnect-PGDatabase when done to ensure that the connection is
    /// properly disposed of.
    /// </remarks>
    [Cmdlet(VerbsCommunications.Connect, "PGDatabase")]
    [OutputType(typeof(NpgsqlConnection))]
    [Alias("cnpgdb")]
    public class ConnectPGDatabaseCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        [ValidateNotNullOrEmpty]
        [Alias("Server")]
        public string ComputerName { get; set; } = "localhost";

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 65535)]
        public int Port { get; set; } = 5432;

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Database { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int CommandTimeout { get; set; } = 30;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int ConnectionIdleLifetime { get; set; } = 300;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int ConnectionPruningInterval { get; set; } = 10;

        [Parameter(Mandatory = false)]
        public bool Multiplexing { get; set; }

        [Parameter(Mandatory = false)]
        public bool Pooling { get; set; } = true;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int Timeout { get; set; } = 15;

        protected override void ProcessRecord()
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = ComputerName,
                    Port = Port,
                    Username = Credential.UserName,
                    Password = Credential.GetNetworkCredential().Password,
                    SslMode = SslMode.Prefer,
                    Pooling = Pooling,
                    MinPoolSize = 0,
                    MaxPoolSize = 100,
                    CommandTimeout = CommandTimeout,
                    ConnectionIdleLifetime = ConnectionIdleLifetime,
                    ConnectionPruningInterval = ConnectionPruningInterval,
                    Timeout = Timeout
                };

                if (MyInvocation.BoundParameters.ContainsKey("Database"))
                    builder.Database = Database;

                if (MyInvocation.BoundParameters.ContainsKey("Multiplexing"))
                    builder.Multiplexing = Multiplexing;

                var conn = new NpgsqlConnection(builder.ConnectionString);
                conn.Open();

                SessionState.PSVariable.Set(new PSVariable(
                    PSModuleConstants.ConnectionVariable,
                    conn,
                    ScopedItemOptions.Private
                ));
                WriteObject(conn);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "DatabaseConnectionFailed", ErrorCategory.ConnectionError, null));
            }
        }
    }
}
