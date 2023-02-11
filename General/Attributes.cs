using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AttributeDataMember : Attribute
    {
        private string mNameColumnDataBase;
        private SQLTypeBasic mSqliteType;

        public AttributeDataMember(string NameColumnDataBase, SQLTypeBasic Type)
        {
            mNameColumnDataBase = NameColumnDataBase;
            mSqliteType = Type;
            AceptaNulos = false;
        }

        public string NameColumnDataBase { get { return mNameColumnDataBase; } }
        public SQLTypeBasic SqliteType { get { return mSqliteType; } }
        public DBNull Nulo { get { return DBNull.Value; } }


        public bool AceptaNulos { get; set; }
        /// <summary>
        /// Tomar en cuenta los siguientes puntos
        /// 1.- Cuando se recupere el valor de la base el primer valor del arreglo es el
        /// que se pondra en la variable en caso de que venga nula
        /// 2.- Cuando se actualice, inserte o borre se tomara en cuenta todos los valores 
        /// del arreglo para definir si el campo es nulo.
        /// </summary>
        public object[] DatosParaNulos { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AttributeDataClass : Attribute
    {
        private string mNameTable;

        public AttributeDataClass(string NameTable)
        {
            mNameTable = NameTable;
        }

        public string NameTable { get { return mNameTable; } }
    }
}
