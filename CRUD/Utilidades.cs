using System;
using System.Reflection;
using General;
using System.Data;

namespace CRUD
{
    internal class Utilidades
    {
        public static EntidadBase LLenaModel(EntidadBase Model, DataRow row)
        {
            try
            {
                PropertyInfo[] ArrayProperty = Model.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                    {
                        if (Attri is AttributeDataMember)
                        {
                            AttributeDataMember atributo = (AttributeDataMember)Attri;
                            object valor = row[atributo.NameColumnDataBase];

                            if (atributo.AceptaNulos && DBNull.Value.Equals(valor))
                            {
                                if (atributo.DatosParaNulos == null && atributo.DatosParaNulos.Length == 0)
                                    throw new Exception(string.Format("La propiedad {0} acepta nulos pero no tiene datos para nulos del atributo", Property.Name));

                                valor = atributo.DatosParaNulos[0];
                            }
                            else
                                valor = Model.RecuperaValor(Property.PropertyType, valor);

                            Property.SetValue(Model, valor, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Model;
        }

        public static EntidadBase LLenaModel(EntidadBase Model, IDataReader row)
        {
            string valiable = "";
            try
            {
                
                PropertyInfo[] ArrayProperty = Model.GetType().GetProperties();

                foreach (PropertyInfo Property in ArrayProperty)
                {
                    Attribute[] ArrayAttribute = Attribute.GetCustomAttributes(Property);

                    foreach (Attribute Attri in ArrayAttribute)
                    {
                        if (Attri is AttributeDataMember)
                        {
                            AttributeDataMember atributo = (AttributeDataMember)Attri;
                            object valor = row[atributo.NameColumnDataBase];
                            valiable = atributo.NameColumnDataBase;
                            
                            if (atributo.AceptaNulos && DBNull.Value.Equals(valor))
                            {
                                if (atributo.DatosParaNulos == null && atributo.DatosParaNulos.Length == 0)
                                    throw new Exception(string.Format("La propiedad {0} acepta nulos pero no tiene datos para nulos del atributo", Property.Name));

                                valor = Convert.ChangeType(atributo.DatosParaNulos[0], Property.PropertyType);

                                if (Property.PropertyType == typeof(decimal))
                                    valor = decimal.Zero;
                            }
                            else
                                valor = Model.RecuperaValor(Property.PropertyType, valor);

                            Property.SetValue(Model, valor, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(valiable);
                throw ex;
            }

            return Model;
        }
    }
}
