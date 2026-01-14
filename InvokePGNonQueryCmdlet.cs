using System;
using System.Management.Automation;
using Npgsql;

namespace PSPostgreSQL
{
    [CmdletBinding()]
    [Cmdlet(VerbsLifecycle.Invoke, "PGNonQuery")]
    [OutputType(typeof(int))]
    [Alias("ipgcmd")]
    public class InvokePGNonQueryCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Query { get; set; }

        [Parameter(Mandatory = false)]
        public NpgsqlConnection Connection { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int CommandTimeout { get; set; } = 30;

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
                    new InvalidOperationException("No connection available. Use Connect-PGDatabase first or provide a Connection."),
                    "NoConnection",
                    ErrorCategory.ConnectionError,
                    null
                ));
                return;
            }

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException($"Connection is not open. Current state: {Connection.State}"),
                    "ConnectionNotOpen",
                    ErrorCategory.ConnectionError,
                    Connection
                ));
                return;
            }

            try
            {
                using (var pgcommand = new NpgsqlCommand(Query, Connection) { CommandTimeout = CommandTimeout })
                {
                    int rowsAffected = pgcommand.ExecuteNonQuery();
                    WriteVerbose($"Query affected {rowsAffected} row(s).");
                    WriteObject(rowsAffected);
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "NonQueryExecutionFailed", ErrorCategory.WriteError, null));
            }
        }
    }
}
