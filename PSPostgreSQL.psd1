@{
    RootModule           = 'PSPostgreSQL.dll'
    ModuleVersion        = '1.0.0'
    GUID                 = '467477c6-4369-43b2-9e2c-598c39c0cd07'
    Author               = 'Phil Silva'
    Copyright            = 'Copyright (c) Phil Silva. All rights reserved.'
    Description          = 'Super simple module for working with PostgreSQL databases'
    CompatiblePSEditions = @('Core')
    PowerShellVersion    = '7.0'
    CmdletsToExport      = @(
        'Connect-PGDatabase'
        'Disconnect-PGDatabase'
        'Invoke-PGQuery'
        'Invoke-PGNonQuery'
    )
    AliasesToExport      = @(
        'cnpgdb'
        'ipgcmd'
        'ipgqry'
        'dcpgdb'
    )
    VariablesToExport    = @(
        'PGSQLConnection'
    )
    PrivateData          = @{
        PSData = @{
            Tags         = @('database', 'postgresql', 'pgsql', 'sql', 'npgsql')
            LicenseUri   = 'https://github.com/badmrspoon/PSPostgreSQL/blob/main/LICENSE'
            ProjectUri   = 'https://github.com/badmrspoon/PSPostgreSQL'
        }
    }
}
