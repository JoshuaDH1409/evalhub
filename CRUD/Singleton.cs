using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRUD.Interfaz;
using CRUD.ADOnetInternal;

namespace CRUD
{
    public sealed class Singleton
    {
        private ICRUD mcrud = null;
        private ICRUDExcel mcrudexcel = null;
        private ICRUDTxt mcrudtext = null;

        private static Singleton mSingleton = null;
        private static readonly object padlock = new object();

        private Singleton()
        {
            string conexion = General.Utilidades.ObtenerSQLServerConexion();
            
            if (string.IsNullOrWhiteSpace(conexion))
                throw new Exception("No se encuentra la conexión a la base de datos.");

            mcrud = new CRUDSqlServer(conexion);
            mcrudexcel = new Excel.OleDbCRUD();
            mcrudtext = new Txt.txt();
        }
        public static Singleton Instance
        {
            get 
            {
                General.Utilidades.PonerCulturaMx();

                if (mSingleton == null)
                {
                    lock (padlock)
                    {
                        if (mSingleton == null)
                            mSingleton = new Singleton();
                    }
                }

                return mSingleton;
            }
        }

        public ICRUD CRUD
        {
            get 
            {
                return mcrud;
            }
        }

        public ICRUDExcel CRUDExcel
        {
            get 
            {
                return mcrudexcel;
            }
        }

        public ICRUDTxt CRUDTxt
        {
            get
            {
                return mcrudtext;
            }
        }
    }
}
