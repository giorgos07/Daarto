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
        /// <param name="dbConnectionStore"></param>
        protected IdentityTable(IDbConnectionStore dbConnectionStore) {
            _dbConnectionStore = dbConnectionStore;
        }

        private readonly IDbConnectionStore _dbConnectionStore;
        private IDbConnection _dbConnection;

        /// <summary>
        /// The type of the database connection class used to access the store.
        /// </summary>
        protected IDbConnection DbConnection 
        {
            get => _dbConnection ??= _dbConnectionStore.GetOrCreateConnection();
        }
    }
}
