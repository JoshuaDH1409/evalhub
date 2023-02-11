using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using General;

namespace Modelo
{
    [AttributeDataClass("vw_evalobjetive")]
    [DataContract]
    public class VEvalObj : EntidadBase
    {
        [DataMember]
        [Display(Name = "Id Periodo")]
        [AttributeDataMember("PeriodId", SQLTypeBasic.INTEGER)]
        public int PeriodId { get; set; }

        [DataMember]
        [Display(Name = "PeriodName")]
        [AttributeDataMember("PeriodName", SQLTypeBasic.TEXT)]
        public string PeriodName { get; set; }

        [DataMember]
        [Display(Name = "CountryId")]
        [AttributeDataMember("CountryId", SQLTypeBasic.INTEGER)]
        public int CountryId { get; set; }

        [DataMember]
        [Display(Name = "Country")]
        [AttributeDataMember("Country", SQLTypeBasic.TEXT)]
        public string Country { get; set; }

        [DataMember]
        [Display(Name = "EvaluatedNoEmp")]
        [AttributeDataMember("EvaluatedNoEmp", SQLTypeBasic.TEXT)]
        public string EvaluatedNoEmp { get; set; }

        [DataMember]
        [Display(Name = "Evaluated")]
        [AttributeDataMember("Evaluated", SQLTypeBasic.TEXT)]
        public string Evaluated { get; set; }

        [DataMember]
        [Display(Name = "Puesto")]
        [AttributeDataMember("Puesto", SQLTypeBasic.TEXT)]
        public string Puesto { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        [AttributeDataMember("Division", SQLTypeBasic.TEXT)]
        public string Division { get; set; }

        [DataMember]
        [Display(Name = "EvaluadorNoEmp")]
        [AttributeDataMember("EvaluadorNoEmp", SQLTypeBasic.TEXT)]
        public string EvaluadorNoEmp { get; set; }

        [DataMember]
        [Display(Name = "Evaluator")]
        [AttributeDataMember("Evaluator", SQLTypeBasic.TEXT)]
        public string Evaluator { get; set; }

        [DataMember]
        [Display(Name = "Objetive")]
        [AttributeDataMember("Objetive", SQLTypeBasic.TEXT)]
        public string Objetive { get; set; }
        [DataMember]
        [Display(Name = "Obj peso")]
        [AttributeDataMember("ObjetivePeso", SQLTypeBasic.INTEGER)]
        public int ObjetivePeso { get; set; }

        [DataMember]
        [Display(Name = "objdesc")]
        [AttributeDataMember("objdesc", SQLTypeBasic.TEXT)]
        public string objdesc { get; set; }

        [DataMember]
        [Display(Name = "objMetricas")]
        [AttributeDataMember("objMetricas", SQLTypeBasic.TEXT)]
        public string objMetricas { get; set; }

        [DataMember]
        [Display(Name = "ObjResult")]
        [AttributeDataMember("ObjResult", SQLTypeBasic.TEXT)]
        public string ObjResult { get; set; }

        [DataMember]
        [Display(Name = "ComentariosJefeEval")]
        [AttributeDataMember("ComentariosJefeEval", SQLTypeBasic.TEXT)]
        public string ComentariosJefeEval { get; set; }

        [DataMember]
        [Display(Name = "ObjResult2")]
        [AttributeDataMember("ObjResult2", SQLTypeBasic.TEXT)]
        public string ObjResult2 { get; set; }

        [DataMember]
        [Display(Name = "ComentariosJefeEval2")]
        [AttributeDataMember("ComentariosJefeEval2", SQLTypeBasic.TEXT)]
        public string ComentariosJefeEval2 { get; set; }

        [DataMember]
        [Display(Name = "AutoEval2")]
        [AttributeDataMember("AutoEval2", SQLTypeBasic.INTEGER)]
        public int AutoEval2 { get; set; }

        [DataMember]
        [Display(Name = "Calif2")]
        [AttributeDataMember("Calif2", SQLTypeBasic.INTEGER)]
        public int Calif2 { get; set; }

        [DataMember]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }
    }
}
