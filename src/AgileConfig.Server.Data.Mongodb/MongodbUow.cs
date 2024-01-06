﻿using MongoDB.Driver;

namespace AgileConfig.Server.Data.Mongodb
{
    /// <summary>
    /// This is a empty implementation of IUow for mongodb.
    /// </summary>
    public class MongodbUow : Abstraction.IUow
    {
        public IClientSessionHandle? Session { get; private set; }

        public async Task<bool> SaveChangesAsync()
        {
            await Session?.CommitTransactionAsync()!;
            return true;
        }

        public void Rollback()
        {
            Session?.AbortTransaction();
        }

        private bool _disposed;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Session?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Begin()
        {
            if (Session?.IsInTransaction != true)
            {
                Session?.StartTransaction();
            }
        }
        
        internal void SetSession(IClientSessionHandle session)
        {
            Session = session;
        }
    }
}