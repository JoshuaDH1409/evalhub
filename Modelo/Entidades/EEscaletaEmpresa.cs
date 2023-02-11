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
    [AttributeDataClass("TBL_Escaleta_EC")]
    [DataContract]
    public class EEscaletaEmpresa : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("ID", SQLTypeBasic.INTEGER)]
        public int ID { get; set; }

        [DataMember]
        [AttributeDataMember("Performance", SQLTypeBasic.TEXT)]
        public string Performance { get; set; }

        [DataMember]
        [AttributeDataMember("Min", SQLTypeBasic.INTEGER)]
        public decimal Min { get; set; }

        [DataMember]
        [AttributeDataMember("Max", SQLTypeBasic.INTEGER)]
        public decimal Max { get; set; }
        [DataMember]
        [AttributeDataMember("Criteria", SQLTypeBasic.INTEGER)]
        public int Criteria { get; set; }

        [DataMember]
        [AttributeDataMember("Score", SQLTypeBasic.INTEGER)]
        public decimal Score { get; set; }
    }
}
