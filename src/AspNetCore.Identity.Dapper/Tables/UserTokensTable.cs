namespace AspNetCore.Identity.Dapper
{
    internal class UserTokensTable
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserTokensTable(IDatabaseConnectionFactory databaseConnectionFactory) => _databaseConnectionFactory = databaseConnectionFactory;
    }
}
