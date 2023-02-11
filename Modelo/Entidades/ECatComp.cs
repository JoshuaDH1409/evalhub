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
    [AttributeDataClass("TBL_CatComp")]
    [DataContract]
   public class ECatComp : EntidadBase
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
            
    }
}
