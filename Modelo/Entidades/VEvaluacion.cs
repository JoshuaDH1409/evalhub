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
    [AttributeDataClass("vw_evaluaciones")]
    [DataContract]
    public class VEvaluacion : EntidadBase
    {
        [DataMember]
        [Display(Name = "Eval Id")]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int Id { get; set; }

        [DataMember]
        [Display(Name = "Periodo")]
        [AttributeDataMember("periodo", SQLTypeBasic.INTEGER)]
        public int Periodo { get; set; }

        [DataMember]
        [Display(Name = "Pais")]
        [AttributeDataMember("Pais", SQLTypeBasic.INTEGER)]
        public int Pais { get; set; }

        [DataMember]
        [Display(Name = "País")]
        [AttributeDataMember("PaisName", SQLTypeBasic.TEXT)]
        public string PaisName { get; set; }

        [DataMember]
        [Display(Name = "EvaluadoId")]
        [AttributeDataMember("id_usuario", SQLTypeBasic.INTEGER)]
        public int UsuarioId { get; set; }

        [DataMember]
        [Display(Name = "Num Empleado")]
        [AttributeDataMember("Expr2", SQLTypeBasic.INTEGER)]
        public int UsuarioSap { get; set; }

        [DataMember]
        [Display(Name = "Evaluado")]
        [AttributeDataMember("NombreCompleto", SQLTypeBasic.TEXT)]
        public string UsuarioNombre { get; set; }
        [DataMember]
        [Display(Name = "Nombre")]
        [AttributeDataMember("Nombre", SQLTypeBasic.TEXT)]
        public string UsuarioNombres { get; set; }
        [DataMember]
        [Display(Name = "A. Paterno")]
        [AttributeDataMember("ApellidoPat", SQLTypeBasic.TEXT)]
        public string UsuarioAPaterno { get; set; }
        [DataMember]
        [Display(Name = "A. Materno")]
        [AttributeDataMember("ApellidoMat", SQLTypeBasic.TEXT)]
        public string UsuarioAMaterno { get; set; }

        [DataMember]
        [Display(Name = "EvaluadorSap")]
        [AttributeDataMember("id_sap", SQLTypeBasic.INTEGER)]
        public int EvaluadorSap { get; set; }

        [DataMember]
        [Display(Name = "Evaluador")]
        [AttributeDataMember("Expr1", SQLTypeBasic.TEXT)]
        public string EvaluadorNombre { get; set; }

        [DataMember]
        [Display(Name = "Nivel")]
        [AttributeDataMember("Nivel", SQLTypeBasic.INTEGER)]
        public int Nivel { get; set; }

        [DataMember]
        [Display(Name = "Puesto")]
        [AttributeDataMember("EvPuesto", SQLTypeBasic.TEXT)]
        public string Puesto { get; set; }

        [DataMember]
        [Display(Name = "División")]
        [AttributeDataMember("EvDivision", SQLTypeBasic.TEXT)]
        public string Division { get; set; }

        //[DataMember]
        //[Display(Name = "Area")]
        //[AttributeDataMember("EvArea", SQLTypeBasic.TEXT)]
        //public string Area { get; set; }

        [DataMember]
        [Display(Name = "Ingreso")]
        [AttributeDataMember("FechaIngreso", SQLTypeBasic.NUMERIC)]
        public long FechaIngreso { get; set; }

        [DataMember]
        [Display(Name = "Comentario Evaluado")]
        [AttributeDataMember("ComEvaluado", SQLTypeBasic.TEXT)]
        public string ComentarioEvaluado { get; set; }

        [DataMember]
        [Display(Name = "Comentario Evaluador")]
        [AttributeDataMember("ComEvaluador", SQLTypeBasic.TEXT)]
        public string ComentarioEvaluador { get; set; }

        [DataMember]
        [Display(Name = "Comentario Segundo Nivel")]
        [AttributeDataMember("ComentariosSegundoNivel", SQLTypeBasic.TEXT)]
        public string ComentariosSegundo { get; set; }

        [DataMember]
        [Display(Name = "Calificación Final")]
        [AttributeDataMember("CaliFinal", SQLTypeBasic.INTEGER)]
        public int CalifFinal { get; set; }

        [DataMember]
        [Display(Name = "Motivo de Calibración")]
        [AttributeDataMember("MotivoCalibracion", SQLTypeBasic.TEXT)]
        public string Calibracion { get; set; }

        [DataMember]
        [Display(Name = "Motivo de Rechazo")]
        [AttributeDataMember("MotivoRechazoObj", SQLTypeBasic.TEXT)]
        public string Rechazo { get; set; }

        [DataMember]
        [Display(Name = "Comentario Evaluado 2 Semestre")]
        [AttributeDataMember("ComEvaluado2", SQLTypeBasic.TEXT)]
        public string ComentarioEvaluado2 { get; set; }

        [DataMember]
        [Display(Name = "Comentario Evaluador 2 Semenstre")]
        [AttributeDataMember("ComEvaluador2", SQLTypeBasic.TEXT)]
        public string ComentarioEvaluador2 { get; set; }

        [DataMember]
        [Display(Name = "Califación Final 2 Sementre")]
        [AttributeDataMember("CaliFinal2", SQLTypeBasic.INTEGER)]
        public int CalifFinal2 { get; set; }

        [DataMember]
        [Display(Name = "Motivo de Calibración 2 Semestre")]
        [AttributeDataMember("MotivoCalibracion2", SQLTypeBasic.TEXT)]
        public string Calibracion2 { get; set; }

        [DataMember]
        [Display(Name = "Motivo de Rechazo 2 Semestre")]
        [AttributeDataMember("MotivoRechazoObj2", SQLTypeBasic.TEXT)]
        public string Rechazo2 { get; set; }

        [DataMember]
        [Display(Name = "Estatus")]
        [AttributeDataMember("stat_name", SQLTypeBasic.TEXT)]
        public string statusName { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        [AttributeDataMember("Status", SQLTypeBasic.INTEGER)]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }

        [DataMember]
        [Display(Name = "Activo")]
        [AttributeDataMember("Expr3", SQLTypeBasic.BLOB)]
        public bool UsuarioActivo { get; set; }

        [DataMember]
        [Display(Name = "Periodo Activo")]
        [AttributeDataMember("PeriodActive", SQLTypeBasic.BLOB)]
        public bool PeriodoActivo { get; set; }
        public string StatusDsc { get; set; }
        [DataMember]
        [Display(Name = "Calificación Final Calibración")]
        [AttributeDataMember("CaliFinalCalibracion", SQLTypeBasic.BLOB)]
        public int CaliFinalCalibracion { get; set; }
    }
}
