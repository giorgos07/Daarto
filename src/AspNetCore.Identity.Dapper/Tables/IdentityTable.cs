using System;
using System.Data;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// A base class for all identity tables.
    /// </summary>
    public class IdentityTable
    {
        /// <summary>
        /// Creates a new instance of <see cref="IdentityTable"/>.
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        public IdentityTable(IDbConnectionFactory dbConnectionFactory) {
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


        private bool _disposed = false;
        /// <inheritdoc/>
        protected virtual void OnDispose() {
            DbConnection?.Dispose();
        }
        /// <inheritdoc/>
        public void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }
            if (disposing) {
                // Free any other managed objects here.
                OnDispose();
            }
            _disposed = true;
        }
    }
}
