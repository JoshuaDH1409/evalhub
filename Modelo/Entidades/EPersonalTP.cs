using General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Modelo
{
    [AttributeDataClass("TBL_objetivesPTP")]
    [DataContract]
    public class EPersonalTP : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        //public int Eval { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Eval es requerido")]
        [Display(Name = "Calificación")]
        [AttributeDataMember("Eval", SQLTypeBasic.INTEGER)]
        public int Eval { get; set; }



        [DataMember]
        [Required(ErrorMessage = "Nombre del objetivo requerido")]
        [Display(Name = "Nombre de Objetivo")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ObjetivePTP", SQLTypeBasic.TEXT)]
        public string ObjetivePTP { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Acción es requerido")]
        [Display(Name = "Acción")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ActionPTP", SQLTypeBasic.TEXT)]
        public string ActionPTP { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Fecha es requerido")]
        [Display(Name = "Fecha de Cumplimiento")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("DateFin", SQLTypeBasic.TEXT)]
        public string DateFin { get; set; }

    }
}
