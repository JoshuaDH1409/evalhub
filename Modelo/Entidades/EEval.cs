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
    [AttributeDataClass("TBL_Eval")]
    [DataContract]
    public class EEval : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "periodo es requerido")]
        [Display(Name = "periodo")]
        [AttributeDataMember("periodo", SQLTypeBasic.INTEGER)]
        public int periodo { get; set; }
        public int periodo_etapa { get; set; }

        [DataMember]
        [Required(ErrorMessage = "id_usuario es requerido")]
        [Display(Name = "id_usuario")]
        [AttributeDataMember("id_usuario", SQLTypeBasic.INTEGER)]
        public int id_usuario { get; set; }

        public string usuarioTemp { get; set; }


        [DataMember]
        [Required(ErrorMessage = "id_evaluador es requerido")]
        [Display(Name = "id_evaluador")]
        [AttributeDataMember("id_evaluador", SQLTypeBasic.TEXT)]
        public string id_evaluador { get; set; }

        public string EvaluadorTemp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Nivel es requerido")]
        [Display(Name = "Nivel")]
        [AttributeDataMember("Nivel", SQLTypeBasic.INTEGER)]
        public int Nivel { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Puesto es requerido")]
        [Display(Name = "Puesto")]
        [AttributeDataMember("EvPuesto", SQLTypeBasic.TEXT)]
        public string Puesto { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Division requerida")]
        [Display(Name = "Division")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("EvDivision", SQLTypeBasic.TEXT)]
        public string Division { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Area requerido")]
        [Display(Name = "Area")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("EvArea", SQLTypeBasic.TEXT)]
        public string Area { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario evaluado requerido")]
        [Display(Name = "Comentario evaluado")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComEvaluado", SQLTypeBasic.TEXT)]
        public string ComEvaluado { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComEvaluador", SQLTypeBasic.TEXT)]
        public string ComEvaluador { get; set; }

        [DataMember]
        [Required(ErrorMessage = "CaliFinal es requerido")]
        [Display(Name = "CaliFinal")]
        [AttributeDataMember("CaliFinal", SQLTypeBasic.INTEGER)]
        public int CaliFinal { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Motivo Calibracion evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("MotivoRechazoObj", SQLTypeBasic.TEXT)]
        public string MotivoRechazoObj { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Motivo Calibracion evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("MotivoCalibracion", SQLTypeBasic.TEXT)]
        public string MotivoCalibracion { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario segundo nivel")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComentariosSegundoNivel", SQLTypeBasic.TEXT)]
        public string ComentariosSegundoNivel { get; set; }



        [DataMember]
        [Required(ErrorMessage = "Comentario evaluado requerido")]
        [Display(Name = "Comentario evaluado")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComEvaluado2", SQLTypeBasic.TEXT)]
        public string ComEvaluado2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComEvaluador2", SQLTypeBasic.TEXT)]
        public string ComEvaluador2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "CaliFinal es requerido")]
        [Display(Name = "CaliFinal")]
        [AttributeDataMember("CaliFinal2", SQLTypeBasic.INTEGER)]
        public int CaliFinal2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Motivo Calibracion evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("MotivoCalibracion2", SQLTypeBasic.TEXT)]
        public string MotivoCalibracion2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "CaliFinal es requerido")]
        [Display(Name = "Cal. Final Calibración")]
        [AttributeDataMember("CaliFinalCalibracion", SQLTypeBasic.INTEGER)]
        public int CaliFinalCalibracion { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Motivo Calibracion evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("MotivoRechazoObj2", SQLTypeBasic.TEXT)]
        public string MotivoRechazoObj2 { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Motivo rechazo evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("MotivoRechazoObjFin", SQLTypeBasic.TEXT)]
        public string MotivoRechazoObjFin { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Motivo Calibracion evaluador requerido")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComentariosSegundoNivel2", SQLTypeBasic.TEXT)]
        public string ComentariosSegundoNivel2 { get; set; }

        
        [DataMember]
        [Required(ErrorMessage = "Status")]
        [Display(Name = "Status")]
        [AttributeDataMember("Status", SQLTypeBasic.NUMERIC)]
        public int Status { get; set; }
        public string StatusDsc { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activo")]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }


        //########Periodo==0###########
        //0-> Pendiente
        //1-> Objetivos cargados
        //2-> objetivos rechazados
        //3-> objetivos aprobados
        //########Periodo==0###########

        //########Periodo==1###########
        //4-> Pendiente
        //5-> Objetivos cargados
        //6-> objetivos rechazados
        //7-> objetivos aprobados
        //8-> objetivos aprobados
        //########Periodo==1###########

        //########Periodo==2###########
        //9-> Inicio Calibración
        //########Periodo==2###########


        //########Periodo==3###########
        //10-> Pendiente
        //11-> Objetivos cargados
        //12-> objetivos rechazados
        //13-> objetivos aprobados
        //14-> Cerrado
        //########Periodo==3###########

        //########Periodo==4###########
        //15-> Inicio Calibración
        //########Periodo==4###########

        //########Periodo!=0###########
        //16-> Pendiente
        //17-> Objetivos cargados
        //18-> objetivos rechazados
        //19-> objetivos aprobados
        //########Periodo!=0###########

        [DataMember]
        [AttributeDataMember("Rechazos", SQLTypeBasic.INTEGER)]
        public int Rechazos { get; set; }
        [DataMember]
        [AttributeDataMember("RechazosMitad", SQLTypeBasic.INTEGER)]
        public int RechazosMitad { get; set; }
        [DataMember]
        [AttributeDataMember("RechazosFin", SQLTypeBasic.INTEGER)]
        public int RechazosFin { get; set; }
        [DataMember]
        [AttributeDataMember("Modificadopor", SQLTypeBasic.TEXT)]
        public string Modificadopor { get; set; }

    }
}
