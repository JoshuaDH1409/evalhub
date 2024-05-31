using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using General;


//MODELO DEL OBJETO

namespace Modelo
{
    [AttributeDataClass("TBL_objetivesPDP")]
    [DataContract]
    public class EPersonalDP : EntidadBase
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
        [Display(Name = "Objetivo")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ObjetivePDP", SQLTypeBasic.TEXT)]
        public string ObjetivePDP { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Acción es requerido")]
        [Display(Name = "Propósito")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ActionPDP", SQLTypeBasic.TEXT)]
        public string actionPDP { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Fecha es requerido")]
        [Display(Name = "¿Cómo podemos Ayudarte?")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("DateFin", SQLTypeBasic.TEXT)]
        public string dateFinish { get; set; }

      
    }
}
