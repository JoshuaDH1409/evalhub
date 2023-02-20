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
    [AttributeDataClass("TBL_CatSubComp")]
    [DataContract]
    public class ECatSubComp : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [AttributeDataMember("idCatComp", SQLTypeBasic.INTEGER)]
        public int idCatComp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Descripcion requerido")]
        [Display(Name = "Descripcion")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("SubCompetencia", SQLTypeBasic.TEXT)]
        public string SubCompetencia { get; set; }



        [DataMember]
        [Required(ErrorMessage = "Descripcion requerido")]
        [Display(Name = "Descripcion")]
        [AttributeDataMember("descripcion", SQLTypeBasic.TEXT)]
        public string Descripcion { get; set; }

    }
}
