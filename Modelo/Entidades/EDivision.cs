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
    [AttributeDataClass("TBL_division")]
    [DataContract]
    public class EDivision : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Display(Name = "División")]
        [AttributeDataMember("Descripcion", SQLTypeBasic.TEXT)]
        public string Descripcion { get; set; }
    }
}
