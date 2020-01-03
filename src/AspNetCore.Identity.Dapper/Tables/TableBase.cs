using System;

namespace AspNetCore.Identity.Dapper
{
    /// <summary>
    /// A base implementation for all tables.
    /// </summary>
    public abstract class TableBase : IDisposable
    {
        private bool _disposed = false;

        /// <inheritdoc/>
        public void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A method to free any other managed objects on classes that onherit from <see cref="TableBase"/>.
        /// </summary>
        protected abstract void OnDispose();

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
