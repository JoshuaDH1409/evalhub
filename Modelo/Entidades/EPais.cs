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
    [AttributeDataClass("TBL_Pais")]
    [DataContract]
   public class EPais : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Descripcion requerido")]
        [Display(Name = "Descripcion")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("descripcion", SQLTypeBasic.TEXT)]
        public string descripcion { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activo")]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }

    }
}
