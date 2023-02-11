using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRUD
{
    public class Cliterio
    {
        public PropertyInfo PropertyColumn { get; set; }

        public OperadoresRelacionales OperadorRelacional { get; set; }

        public object Valor { get; set; }

        public TipoValor Tipo { get; set; }

        public OperadoresLogicos OperadorLogico { get; set; }

        public Cliterio(PropertyInfo propertycolumn, OperadoresRelacionales operadorelacional, object valor, TipoValor tipo)
        {
            PropertyColumn = propertycolumn;
            OperadorRelacional = operadorelacional;
            Valor = valor;
            Tipo = tipo;
            OperadorLogico = OperadoresLogicos.Noaplica;
        }

        public Cliterio(OperadoresLogicos operadorlogico, PropertyInfo propertycolumn, OperadoresRelacionales operadorelacional, object valor, TipoValor tipo)
        {
            PropertyColumn = propertycolumn;
            OperadorRelacional = operadorelacional;
            Valor = valor;
            Tipo = tipo;
            OperadorLogico = operadorlogico;
        }

        public override string ToString()
        {
            return string.Format("Cliterio:\n operadorlogico: {0} propertycolumn: {1} operadorelacional: {2} valor: {3} tipo: {4}", OperadorLogico.ToString(),
                                                                                                                                  PropertyColumn.ToString(),
                                                                                                                                  OperadorRelacional.ToString(),
                                                                                                                                  Valor.ToString(),
                                                                                                                                  Tipo.ToString());
        }
    }
}
