namespace GakkoServices.Core.Configuration
{
    public abstract partial class DatabaseConfiguration
    {
        public enum SupportedDatabaseServerEngines
        {
            None = 0,
            MSSQL = 1,
            PostgreSQL = 2,
            MySQL = 3,
            SQLite = 4
        }
    }
}
