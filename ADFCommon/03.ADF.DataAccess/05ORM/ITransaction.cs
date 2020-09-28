using System;
using System.Data;

namespace ADF.DataAccess.ORM
{
    public interface ITransaction
    {
        ITransaction BeginTransaction();
        ITransaction BeginTransaction(IsolationLevel level);
        void CommitTransaction();
        void RollbackTransaction();
        (bool success, Exception exception) EndTransaction();
    }
}