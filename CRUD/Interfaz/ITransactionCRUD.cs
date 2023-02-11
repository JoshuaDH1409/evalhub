using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRUD.Transaction
{
    public interface ITransactionCRUD: IDisposable
    {
        void Commit();

        void Rollback();
    }
}
