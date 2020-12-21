using System;
using System.Data;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// A base class for all identity tables.
    /// </summary>
    public abstract class IdentityTable
    {
        /// <summary>
        /// Creates a new instance of <see cref="IdentityTable"/>.
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        protected IdentityTable(IDbConnectionFactory dbConnectionFactory) {
            _dbConnectionFactory = dbConnectionFactory;
        }

        private IDbConnectionFactory _dbConnectionFactory;
        private IDbConnection _dbConnection;

        /// <summary>
        /// The type of the database connection class used to access the store.
        /// </summary>
        protected IDbConnection DbConnection 
        {
            get 
            {
                if (_dbConnection == null) {
                    _dbConnection = _dbConnectionFactory.GetOrCreateConnection();
                    _dbConnectionFactory = null;
                }
                return _dbConnection;
            }
        }
    }
}
