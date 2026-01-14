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
    [CmdletBinding()]
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
                    Pooling = true,
                    MinPoolSize = 0,
                    MaxPoolSize = 100,
                    ConnectionIdleLifetime = 300,
                    ConnectionPruningInterval = 10
                };

                if (MyInvocation.BoundParameters.ContainsKey("Database"))
                    builder.Database = Database;

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
