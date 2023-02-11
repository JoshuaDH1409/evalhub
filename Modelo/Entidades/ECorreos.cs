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
    [AttributeDataClass("TBL_Correos")]
    [DataContract]
   public class ECorreos: EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Asunto requerido")]
        [Display(Name = "Asunto")]
        [StringLength(200, ErrorMessage = "El campo no puede tener más de 150 letras.")]
        [AttributeDataMember("asunto", SQLTypeBasic.TEXT)]
        public string asunto { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Mensaje requerido")]
        [Display(Name = "Mensaje")]
        [StringLength(1200, ErrorMessage = "El campo no puede tener más de 550 letras.")]
        [AttributeDataMember("Mensaje", SQLTypeBasic.TEXT)]
        public string Mensaje { get; set; }

        [DataMember]
        [AttributeDataMember("Paso", SQLTypeBasic.INTEGER)]
        public int Paso { get; set; }

    }
}
