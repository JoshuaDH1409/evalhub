using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using General;

namespace Modelo
{
    [AttributeDataClass("TBL_Escaleta")]
    [DataContract]
    public class EEscaleta : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("ID", SQLTypeBasic.INTEGER)]
        public int Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Objetivo")]
        [AttributeDataMember("Cal_Objetivo", SQLTypeBasic.TEXT)]
        public string Objetivo { get; set; }

        [DataMember]
        [Display(Name = "Valor")]
        [AttributeDataMember("Cal_Valor", SQLTypeBasic.INTEGER)]
        public int Valor { get; set; }

        [DataMember]
        [Display(Name = "Ponderación")]
        [AttributeDataMember("Cal_Ponderacion", SQLTypeBasic.TEXT)]
        public string Ponderacion { get; set; }
        [DataMember]
        [Display(Name = "Ponderación Min")]
        [AttributeDataMember("Cal_Min", SQLTypeBasic.INTEGER)]
        public int Cal_Min { get; set; }

        [DataMember]
        [Display(Name = "Ponderación Max")]
        [AttributeDataMember("Cal_Max", SQLTypeBasic.INTEGER)]
        public int Cal_Max { get; set; }

        [DataMember]
        [AttributeDataMember("Cal_Bono", SQLTypeBasic.INTEGER)]
        public int Bono { get; set; }
    }
}
