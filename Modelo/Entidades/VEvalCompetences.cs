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
    [AttributeDataClass("vw_evalcompetences")]
    [DataContract]
    public class VEvalCompetences : EntidadBase
    {
        [DataMember]
        [Display(Name = "Id Sap")]
        [AttributeDataMember("id_sap", SQLTypeBasic.INTEGER)]
        public int UserSapId { get; set; }

        [DataMember]
        [Display(Name = "Nombre Empleado")]
        [AttributeDataMember("UserName", SQLTypeBasic.TEXT)]
        public string UserName { get; set; }

        [DataMember]
        [Display(Name = "Area")]
        [AttributeDataMember("Area", SQLTypeBasic.TEXT)]
        public string Area { get; set; }

        [DataMember]
        [Display(Name = "División")]
        [AttributeDataMember("Division", SQLTypeBasic.TEXT)]
        public string Division { get; set; }

        [DataMember]
        [Display(Name = "Nombre de la competencia")]
        [AttributeDataMember("CatCompName", SQLTypeBasic.TEXT)]
        public string NombreCatCompetencia { get; set; }

        [DataMember]
        [Display(Name = "Actividades de la competencia")]
        [AttributeDataMember("CompActivities", SQLTypeBasic.TEXT)]
        public string ActividadesCompetence { get; set; }

        [DataMember]
        [Display(Name = "Resultados de la competencia")]
        [AttributeDataMember("CompResults", SQLTypeBasic.TEXT)]
        public string ResultadosCompetence { get; set; }

        [DataMember]
        [Display(Name = "Periodo")]
        [AttributeDataMember("NombrePais", SQLTypeBasic.TEXT)]
        public string PaisNombre { get; set; }

        [DataMember]
        [Display(Name = "Id de país")]
        [AttributeDataMember("PaisId", SQLTypeBasic.INTEGER)]
        public int PaisId { get; set; }

        [DataMember]
        [Display(Name = "Id de evaluacion")]
        [AttributeDataMember("EvalId", SQLTypeBasic.INTEGER)]
        public int EvalId { get; set; }

        [DataMember]
        [Display(Name = "Id del periodo")]
        [AttributeDataMember("PeriodoId", SQLTypeBasic.INTEGER)]
        public int PeriodoId { get; set; }

        [DataMember]
        [Display(Name = "Id de competencia en catalogo")]
        [AttributeDataMember("CatCompId", SQLTypeBasic.INTEGER)]
        public int CompCatId { get; set; }

        [DataMember]
        [Display(Name = "Id de la competencia")]
        [AttributeDataMember("CompId", SQLTypeBasic.INTEGER)]
        public int CompId { get; set; }

        [DataMember]
        [Display(Name = "Usuario Activo")]
        [AttributeDataMember("UsuarioActivo", SQLTypeBasic.BLOB)]
        public bool UsuarioActivo { get; set; }
    }
}
