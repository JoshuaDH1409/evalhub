using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using General;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelo
{
    [AttributeDataClass("TBL_Objetivo_Pais")]
    [DataContract]
    public class EObjetivoPais : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("ID", SQLTypeBasic.INTEGER)]
        public int ID { get; set; }

        [DataMember]
        [AttributeDataMember("Periodo_Id", SQLTypeBasic.INTEGER)]
        public int Periodo_Id { get; set; }

        [DataMember]
        [AttributeDataMember("Pais_Id", SQLTypeBasic.INTEGER)]
        public int Pais_Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Objetivo")]
        [AttributeDataMember("Objetivo_Dsc", SQLTypeBasic.TEXT)]
        [DataType(DataType.MultilineText)]
        public string Objetivo_Dsc { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Peso ponderado %")]
        [Range(0, 100, ErrorMessage = "Ingrese solo números enteros")]
        [AttributeDataMember("Objetivo_Peso", SQLTypeBasic.INTEGER)]
        public int Objetivo_Peso { get; set; }

        [DataMember]
        [Display(Name = "Cumplimiento %")]
        [AttributeDataMember("Objetivo_Cumplimiento", SQLTypeBasic.INTEGER)]
        [Range(0, 100, ErrorMessage = "Ingrese solo números enteros")]
        public int Objetivo_Cumplimiento { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString ="{0:F1}")]
        public decimal Valor {
            get
            {
                return Decimal.Divide(Objetivo_Peso * Objetivo_Cumplimiento,100);
            }
        }

        [DataMember]
        [AttributeDataMember("Objetivo_Cerrado", SQLTypeBasic.BLOB)]
        public bool Objetivo_Cerrado { get; set; }
    }
}
