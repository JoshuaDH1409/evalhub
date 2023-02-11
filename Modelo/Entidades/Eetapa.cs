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
    [AttributeDataClass("TBL_Etapa")]
    [DataContract]
    public class Eetapa : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("Etapa_Id", SQLTypeBasic.INTEGER)]
        public int Etapa { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Etapa_Dsc")]
        [AttributeDataMember("Etapa_Dsc", SQLTypeBasic.TEXT)]
        public string EtapaDsc { get; set; }
    }
}
