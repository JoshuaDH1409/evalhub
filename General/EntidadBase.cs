using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace General
{
    [DataContract]
    public abstract class EntidadBase: ICloneable
    {
        public EntidadBase()
        {
            Operacion = TipoOperacion.Lectura;
        }

        public TipoOperacion Operacion { get; set; }

        public override string ToString()
        {
            try
            {
                PropertyInfo[] ArrayProperty = this.GetType().GetProperties();
                string result = string.Empty;

                foreach (PropertyInfo Property in ArrayProperty)
                    if(Property.CanRead)
                        result = string.Format("{0}{1}: {2}, ",result, Property.Name, Property.GetValue(this, null));

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public virtual EntidadBase NuevoObjeto()
        {
            try
            {
                ConstructorInfo tmp = this.GetType().GetConstructor(new Type[] { });

                return (EntidadBase)tmp.Invoke(null);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            
        }

        public virtual object Clone()
        {
            try
            {
                PropertyInfo[] ArrayProperty = this.GetType().GetProperties();

                EntidadBase result = this.NuevoObjeto();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    if (Property.CanWrite && Property.CanRead)
                    {
                        object valor = Property.GetValue(this, null);
                        Property.SetValue(result, valor, null);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object RecuperaValor(Type type, object Valor)
        {

            if (type == typeof(int))
                return ConvertObjectToInt(Valor);

            if (type == typeof(string))
                return ConvertObjectToString(Valor);

            if (type == typeof(bool))
                return ConvertObjectToBool(Valor);

            if (type == typeof(long))
                return ConvertObjectToLong(Valor);

            if (type == typeof(decimal))
                return ConvertObjectToDecimal(Valor);

            if (type == typeof(short))
                return ConvertObjectToShort(Valor);

            if (type == typeof(TipoOperacion))
                return (TipoOperacion)Valor;

            throw new Exception("Tipo de dato no encontrado");
        }

        private int ConvertObjectToInt(object Valor)
        {
            int result = 0;

            if (DBNull.Value.Equals(Valor))
                return 0;
            else
                return int.TryParse(Valor.ToString(), out result) ? result : 0;
        }

        private bool ConvertObjectToBool(object Valor)
        {
            if (DBNull.Value.Equals(Valor))
                return false;
            else
                if (Valor is bool)
                    return (bool)Valor;
                else
                    return Valor.ToString() == "1" ? true : false;
        }

        private string ConvertObjectToString(object Valor)
        {
            if (DBNull.Value.Equals(Valor))
                return string.Empty;
            else
                return (string)Valor;
        }

        private long ConvertObjectToLong(object Valor)
        {
            long result = 0;

            if (DBNull.Value.Equals(Valor))
                return 0;
            else
                return long.TryParse(Valor.ToString(), out result) ? result : 0;
        }

        private decimal ConvertObjectToDecimal(object Valor)
        {
            decimal result = 0;

            if (DBNull.Value.Equals(Valor))
                return 0;
            else
                return decimal.TryParse(Valor.ToString(), out result) ? result : 0;
        }

        private short ConvertObjectToShort(object Valor)
        {
            short result = 0;

            if (DBNull.Value.Equals(Valor))
                return 0;
            else
                return short.TryParse(Valor.ToString(), out result) ? result : (short)0;
        }
    }
}
