using System;
using System.Management.Automation;
using Npgsql;

namespace PSPostgreSQL
{
    [CmdletBinding()]
    [Cmdlet(VerbsCommunications.Disconnect, "PGDatabase")]
    [OutputType(typeof(NpgsqlConnection))]
    [Alias("dcpgdb")]
    public class DisconnectPGDatabaseCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true)]
        public NpgsqlConnection Connection { get; set; }

        protected override void BeginProcessing()
        {
            if (Connection == null)
                Connection = (NpgsqlConnection)SessionState.PSVariable.GetValue(PSModuleConstants.ConnectionVariable);
        }

        protected override void ProcessRecord()
        {
            if (Connection == null)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException("No connection to disconnect."),
                    "NoConnection",
                    ErrorCategory.InvalidOperation,
                    null
                ));
                return;
            }

            try
            {
                if (Connection.State != System.Data.ConnectionState.Closed)
                    Connection.Close();

                // Properly dispose of the connection
                Connection.Dispose();
                WriteVerbose("Database connection closed and disposed.");
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "DatabaseDisconnectionFailed", ErrorCategory.CloseError, Connection));
            }
            finally
            {
                SessionState.PSVariable.Remove(PSModuleConstants.ConnectionVariable);
            }
        }
    }
}
