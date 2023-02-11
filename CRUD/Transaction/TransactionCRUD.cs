using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRUD.Interfaz;
using System.Transactions;
using System.Data.Common;
using System.Data;

namespace CRUD.Transaction
{
    internal sealed class TransactionCRUD : ITransactionCRUD
    {
        internal DbDataAdapter mDataAdapter = null;

        internal TransactionScope mScope = null;

        internal DataAdapter DataAdapter { get { return mDataAdapter; } }

        internal TransactionCRUD()
        { }

        internal TransactionCRUD(DbDataAdapter dataadapter)
        {
            try
            {
                mDataAdapter = dataadapter;
                mScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TimeSpan(1, 0, 0));
                if (dataadapter.SelectCommand.Connection.State == ConnectionState.Open)
                    mDataAdapter.SelectCommand.Connection.Close();
                mDataAdapter.SelectCommand.Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Commit()
        {
            try
            {
                if (mScope != null)
                {
                    mScope.Complete();
                    mScope.Dispose();
                    mScope = null;
                    mDataAdapter.SelectCommand.Connection.Close();
                    mDataAdapter = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Rollback()
        {
            try
            {
                if (mScope != null)
                {
                    mScope.Dispose();
                    mScope = null;
                    mDataAdapter.SelectCommand.Connection.Close();
                    mDataAdapter = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (mDataAdapter != null)
                Rollback();
        }
    }
}
