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
    [AttributeDataClass("TBL_Status")]
    [DataContract]
    public class EStatus : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("stat_id", SQLTypeBasic.INTEGER)]
        public int Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Nombre")]
        [AttributeDataMember("stat_name", SQLTypeBasic.TEXT)]
        public string Name { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Descripción")]
        [AttributeDataMember("stat_desc", SQLTypeBasic.TEXT)]
        public string Descript { get; set; }

        [DataMember]
        [Display(Name = "Activo")]
        [AttributeDataMember("stat_active", SQLTypeBasic.BLOB)]
        public bool Active { get; set; }

        [DataMember]
        [AttributeDataMember("stat_porc", SQLTypeBasic.INTEGER)]
        public int Porc { get; set; }
    }
}
