using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRUD.Interfaz;
using General;
using System.Reflection;
using System.Data;
using CRUD.Transaction;

namespace CRUD.ADOnetInternal
{
    internal abstract class CRUD : ICRUD
    {
        private const string mSelect = "SELECT {0} FROM {1};";
        private const string mSelectWhere = "SELECT {0} FROM {1} WHERE {2};";
        private const string mSelectMax = "SELECT MAX({0}) FROM {1};";
        private const string mSelectMaxWhere = "SELECT MAX({0}) FROM {1} WHERE {2};";
        private const string mInsert = "INSERT INTO {0} ({1}) VALUES ({2});";
        private const string mUpdate = "UPDATE {0} WITH (ROWLOCK,XLOCK) SET {1} WHERE {2};";
        private const string mDeleteWhere = "DELETE FROM {0} WHERE {1};";
        private const string mDelete = "DELETE FROM {0};";

        protected string mConnectionString;

        protected CRUD(string ConnectionString)
        {
            mConnectionString = ConnectionString;
        }

        public abstract ITransactionCRUD BeginsTransaction();
        public abstract List<EntidadBase> Read(EntidadBase entidad, List<Cliterio> cliterios);
        public abstract List<EntidadBase> Read(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> cliterios);
        public abstract List<EntidadBase> ReadAll(EntidadBase entidad);
        public abstract List<EntidadBase> ReadAll(ITransactionCRUD tran, EntidadBase entidad);
        public abstract bool Save(EntidadBase entidad);
        public abstract bool Save(ITransactionCRUD tran, EntidadBase entidad);
        public abstract bool Update(EntidadBase entidad, List<Cliterio> Cliterio);
        public abstract bool Update(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio);
        public abstract bool delete(EntidadBase entidad);
        public abstract bool delete(ITransactionCRUD tran, EntidadBase entidad);
        public abstract bool delete(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio);
        public abstract object MaxId(EntidadBase entidad, PropertyInfo propertycolumn);
        public abstract object MaxId(EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio);
        public abstract object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn);
        public abstract object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio);
        public abstract bool deleteAllData(ITransactionCRUD tran, EntidadBase entidad);
        public abstract bool BulkCopy(ITransactionCRUD tran, List<EntidadBase> listentidad);
        public abstract string SaveRecuperaSentencia(EntidadBase entidad);
        public abstract string UpdateRecuperaSentencia(EntidadBase entidad, List<Cliterio> Cliterio);
        public abstract bool ExecSentencia(ITransactionCRUD tran, string sentencia);
        public abstract bool ExecSentencia(string sentencia);

        #region Metodos Protected
        protected string Select(EntidadBase Model, List<Cliterio> Cliterio)
        {
            try
            {
                string Columns = GetColumn(Model);
                string TableName = GetTableName(Model);
                string Where = GetWhere(Model, Cliterio);

                if (string.IsNullOrWhiteSpace(Columns) || string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Where))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mSelectWhere, Columns, TableName, Where);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Select(EntidadBase Model)
        {
            try
            {
                string Columns = GetColumn(Model);
                string TableName = GetTableName(Model);

                if (string.IsNullOrWhiteSpace(Columns) || string.IsNullOrWhiteSpace(TableName))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mSelect, Columns, TableName);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Select(EntidadBase Model, PropertyInfo propertycolumn)
        {
            try
            {
                string Columns = ObtenerNombreColumna(propertycolumn);
                string TableName = GetTableName(Model);

                if (string.IsNullOrWhiteSpace(Columns) || string.IsNullOrWhiteSpace(TableName))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mSelectMax, Columns, TableName);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Select(EntidadBase Model, PropertyInfo propertycolumn, List<Cliterio> Cliterio)
        {
            try
            {
                string Columns = ObtenerNombreColumna(propertycolumn);
                string TableName = GetTableName(Model);
                string Where = GetWhere(Model, Cliterio);

                if (string.IsNullOrWhiteSpace(Columns) || string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Where))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mSelectMaxWhere, Columns, TableName, Where);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Insert(EntidadBase Model)
        {
            try
            {
                string[] Values = GetColumnAndValues(Model);
                string TableName = GetTableName(Model);

                if (string.IsNullOrWhiteSpace(Values[0]) || string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Values[1]))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mInsert, TableName, Values[0], Values[1]);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string UpdateBase(EntidadBase entidad, List<Cliterio> Cliterio)
        {
            try
            {
                string TableName = GetTableName(entidad);
                string Columns = UpdateSet(entidad);
                string Where = GetWhere(entidad, Cliterio);

                if (string.IsNullOrWhiteSpace(Columns) || string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Where))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mUpdate, TableName, Columns, Where);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Delete(EntidadBase entidad)
        {
            try
            {
                string TableName = GetTableName(entidad);
                string Where = GetWhere(entidad, ObtenCliteriosDelModelo(entidad));

                if (string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Where))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mDeleteWhere, TableName, Where);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string Delete(EntidadBase entidad, List<Cliterio> Cliterio)
        {
            try
            {
                string TableName = GetTableName(entidad);
                string Where = GetWhere(entidad, Cliterio); 

                if (string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(Where))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mDeleteWhere, TableName, Where);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string DeleteAll(EntidadBase entidad)
        {
            try
            {
                string TableName = GetTableName(entidad);

                if (string.IsNullOrWhiteSpace(TableName))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return string.Format(mDelete, TableName);
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected DataTable GeneraDatatable(List<EntidadBase> listentidad)
        {
            try
            {
                DataTable dt = GeneraSquema(listentidad[0]);

                foreach (EntidadBase entidad in listentidad)
                {
                    PropertyInfo[] ArrayProperty = entidad.GetType().GetProperties();
                    DataRow row = dt.NewRow();

                    foreach (PropertyInfo Property in ArrayProperty)
                    {
                        Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                        foreach (Attribute Attri in ArrayAttribute)
                        {
                            if (Attri is AttributeDataMember)
                            {
                                AttributeDataMember atributo = (AttributeDataMember)Attri;
                                object dato = Property.GetValue(entidad, null);
                                bool nulo = false;

                                nulo = EsNUll(atributo, dato);

                                if (nulo)
                                    dato = DBNull.Value;

                                row[((AttributeDataMember)Attri).NameColumnDataBase] = dato;
                            }
                        }
                    }

                    dt.Rows.Add(row);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        protected string NombreTabla(EntidadBase entidad)
        {
            try
            {
                string TableName = GetTableName(entidad);

                if (string.IsNullOrWhiteSpace(TableName))
                    throw new Exception("No se pudo recuperar la sentencia a ejecutar");

                return TableName;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        #endregion

        #region Metodos Privados Sentencias
        private string GetColumn(EntidadBase Model)
        {
            try
            {
                string Columns = string.Empty;
                PropertyInfo[] ArrayProperty = Model.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                        if (Attri is AttributeDataMember)
                            Columns += string.Format("[{0}],", ((AttributeDataMember)Attri).NameColumnDataBase);
                }

                Columns = Columns.Substring(0, Columns.Length - 1);

                return Columns;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string GetTableName(EntidadBase Model)
        {
            try
            {
                string TableName = string.Empty;
                Attribute[] ArrayAttributeClass = Attribute.GetCustomAttributes(Model.GetType());

                foreach (Attribute Attri in ArrayAttributeClass)
                    if (Attri is AttributeDataClass)
                    {
                        TableName = string.Format("[{0}]", ((AttributeDataClass)Attri).NameTable);
                        break;
                    }

                return TableName;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string GetWhere(EntidadBase Model, List<Cliterio> Cliterio)
        {
            string Where = string.Empty;
            try
            {
                if (Cliterio == null)
                    throw new Exception("No se encontraron cliterios para definir el where");

                if (Cliterio.FindAll(x => x.OperadorLogico == OperadoresLogicos.Noaplica).Count == 1)
                {
                    foreach (Cliterio item in Cliterio)
                    {
                        string OperadorLogico, OperadorRelacionalConValor, NameColumn;
                        object valor;

                        OperadorLogico = ObtenOperadorLogico(item.OperadorLogico);
                        valor = ObtenFormatoValor(item.Tipo, item.Valor);
                        OperadorRelacionalConValor = ObtenOperadorRelacionalConValor(item.OperadorRelacional, valor);
                        NameColumn = ObtenerNombreColumna(item.PropertyColumn);

                        if (item.OperadorRelacional == OperadoresRelacionales.ANDLOGICO)
                            Where += string.Format("{0} ({1} {2} ", OperadorLogico, NameColumn, OperadorRelacionalConValor);
                        else
                            Where += string.Format("{0} {1} {2} ", OperadorLogico, NameColumn, OperadorRelacionalConValor);
                    }
                }
                else
                    throw new Exception("Los cliterios no tiene el formato adecuado");

                return Where;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string[] GetColumnAndValues(EntidadBase Model)
        {
            try
            {
                string[] result = new string[] { string.Empty, string.Empty };

                PropertyInfo[] ArrayProperty = Model.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                        if (Attri is AttributeDataMember)
                        {
                            AttributeDataMember atributo = (AttributeDataMember)Attri;

                            result[0] += string.Format("[{0}],", atributo.NameColumnDataBase);
                            object dato = Property.GetValue(Model, null);
                            bool nulo = false;

                            nulo = EsNUll(atributo, dato);

                            if (nulo)
                                dato = "NULL";
                            else
                                dato = ObtenValorDePropiedad(dato, atributo.SqliteType);

                            result[1] += string.Format("{0},", dato);
                        }
                }

                result[0] = result[0].Substring(0, result[0].Length - 1);
                result[1] = result[1].Substring(0, result[1].Length - 1);

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string UpdateSet(EntidadBase Model)
        {
            try
            {
                string result = string.Empty;

                PropertyInfo[] ArrayProperty = Model.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                        if (Attri is AttributeDataMember)
                        {
                            AttributeDataMember atributo = (AttributeDataMember)Attri;
                            object dato = Property.GetValue(Model, null);
                            bool nulo = false;

                            nulo = EsNUll(atributo, dato);

                            if (nulo)
                                dato = "NULL";
                            else
                                dato = ObtenValorDePropiedad(dato, atributo.SqliteType);

                            result += string.Format("[{0}] = {1},", ((AttributeDataMember)Attri).NameColumnDataBase, dato);
                        }
                }

                result = result.Substring(0, result.Length - 1);

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private DataTable GeneraSquema(EntidadBase entidad)
        {
            try
            {
                DataTable dt = new DataTable();

                PropertyInfo[] ArrayProperty = entidad.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)                    
                        if (Attri is AttributeDataMember)
                            dt.Columns.Add(new DataColumn(((AttributeDataMember)Attri).NameColumnDataBase, Property.PropertyType));
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        #endregion

        #region Metodos Privados Auxiliares
        private string ObtenOperadorLogico(OperadoresLogicos Operador)
        {
            try
            {
                string result;
                switch (Operador)
                {
                    case OperadoresLogicos.Noaplica:
                        result = string.Empty;
                        break;
                    case OperadoresLogicos.AND:
                        result = "AND";
                        break;
                    case OperadoresLogicos.OR:
                        result = "OR";
                        break;
                    default:
                        result = string.Empty;
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private object ObtenFormatoValor(TipoValor Tipo, object Valor)
        {
            try
            {
                object result;

                if (Valor.ToString() == "NULL")
                    result = "NULL";
                else
                {
                    switch (Tipo)
                    {
                        case TipoValor.Texto:
                            result = string.Format("'{0}'", Valor.ToString().Replace("'", "''"));
                            break;
                        case TipoValor.Numero:
                            result = Valor;
                            break;
                        case TipoValor.Boleano:
                            result = (bool)Valor ? 1 : 0;
                            break;
                        case TipoValor.Fecha:
                            result = ((DateTime)Valor).Ticks;
                            break;
                        case TipoValor.ParaIN:
                            result = Valor;
                            break;
                        default:
                            result = string.Empty;
                            break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string ObtenOperadorRelacionalConValor(OperadoresRelacionales Operador, object valor)
        {
            try
            {
                string result;
                if (valor.ToString() == "NULL")
                    result = string.Format(" is {0}", valor);
                else
                {
                    switch (Operador)
                    {
                        case OperadoresRelacionales.LIKE:
                            result = string.Format("LIKE '%{0}%'", valor.ToString().Remove(valor.ToString().Length - 1, 1).Remove(0, 1));
                            break;
                        case OperadoresRelacionales.IGUAL:
                            result = string.Format("= {0}", valor);
                            break;
                        case OperadoresRelacionales.MAYORQUE:
                            result = string.Format("> {0}", valor);
                            break;
                        case OperadoresRelacionales.MENORQUE:
                            result = string.Format("< {0}", valor);
                            break;
                        case OperadoresRelacionales.MAYORIGUAL:
                            result = string.Format(">= {0}", valor);
                            break;
                        case OperadoresRelacionales.MENORIGUAL:
                            result = string.Format("<= {0}", valor);
                            break;
                        case OperadoresRelacionales.IN:
                            result = string.Format("IN ({0})", valor);
                            break;
                        case OperadoresRelacionales.DIFERENTE:
                            result = string.Format("<> {0}", valor);
                            break;
                        case OperadoresRelacionales.NOTIN:
                            result = string.Format("NOT IN ({0})", valor);
                            break;
                        case OperadoresRelacionales.ANDLOGICO:
                            result = string.Format("& {0}) = {0}", valor);
                            break;
                        default:
                            result = string.Empty;
                            break;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private string ObtenerNombreColumna(PropertyInfo Property)
        {
            try
            {
                string resul = string.Empty;

                Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                foreach (Attribute Attri in ArrayAttribute)
                    if (Attri is AttributeDataMember)
                        resul = string.Format("{0}", ((AttributeDataMember)Attri).NameColumnDataBase);

                if (string.IsNullOrWhiteSpace(resul))
                    throw new Exception("No se pudor recuperar la información de la columna.");

                return resul;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private object ObtenValorDePropiedad(object value, SQLTypeBasic tipo)
        {
            try
            {
                object valor;
                int a = 0;

                switch (tipo)
                {
                    case SQLTypeBasic.INTEGER:
                        if (value is bool)
                            valor = (bool)value ? 1 : 0;
                        else
                            valor = int.TryParse(value.ToString(), out a) ? a : 0;
                        break;
                    case SQLTypeBasic.REAL:
                    case SQLTypeBasic.NUMERIC:
                        valor = value;
                        break;
                    case SQLTypeBasic.BLOB:
                    case SQLTypeBasic.TEXT:
                        if(value == null)
                            valor = "NULL";
                        else
                            valor = string.Format("'{0}'", value.ToString().Replace("'", "''"));
                        break;
                    default:
                        valor = "NULL";
                        break;
                }

                return valor;

            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private List<Cliterio> ObtenCliteriosDelModelo(EntidadBase entidad)
        {
            try
            {
                List<Cliterio> result = new List<Cliterio>();
                bool primerdato = true;

                PropertyInfo[] ArrayProperty = entidad.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                    {
                        if (Attri is AttributeDataMember)
                        {
                            TipoValor tipo = TipoValor.Boleano;
                            switch (((AttributeDataMember)Attri).SqliteType)
                            {
                                case SQLTypeBasic.REAL:
                                case SQLTypeBasic.INTEGER:
                                case SQLTypeBasic.NUMERIC:
                                    tipo = TipoValor.Numero;
                                    break;
                                case SQLTypeBasic.TEXT:
                                case SQLTypeBasic.BLOB:
                                    tipo = TipoValor.Texto;
                                    break;
                            }
                            
                            AttributeDataMember atributo = (AttributeDataMember)Attri;
                            object valor = Property.GetValue(entidad, null);
                            bool nulo = false;

                            nulo = EsNUll(atributo, valor);

                            if (nulo)
                                valor = "NULL";
                            else
                            {
                                if (valor is bool)
                                    valor = (bool)valor ? 1 : 0;
                            }

                            if (primerdato)
                            {
                                result.Add(new Cliterio(Property, OperadoresRelacionales.IGUAL, valor, tipo));
                                primerdato = false;
                            }
                            else
                                result.Add(new Cliterio(OperadoresLogicos.AND, Property, OperadoresRelacionales.IGUAL, valor, tipo));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        private bool EsNUll(AttributeDataMember atributo, object dato)
        {
            try
            {
                if (atributo.AceptaNulos)
                {
                    if (atributo.DatosParaNulos == null && atributo.DatosParaNulos.Length == 0)
                        throw new Exception(string.Format("La propiedad {0} acepta nulos pero no tiene datos para nulos del atributo", atributo.NameColumnDataBase));

                    foreach (object item in atributo.DatosParaNulos)
                    {
                        if (item == null)
                        {
                            if (item == dato)
                                return true;
                        }
                        else if (item.ToString() == dato.ToString())
                            return true;
                        else if (dato is decimal)
                        {
                            if (Convert.ToDecimal(item) == (decimal)dato)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                                   new string[] { string.Format("Error: {0}", ex.Message),
                                                                  string.Format("InnerException: {0}", ex.InnerException)});
            }
            return false;
        }
        #endregion
    }
}
