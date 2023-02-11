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
    [AttributeDataClass("TBL_Periodo")]
    [DataContract]
    public class EPeriodos : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Nombre del Periodo es requerido")]
        [Display(Name = "Nombre del Periodo")]
        [StringLength(80, ErrorMessage = "El campo no puede tener más de 80 letras.")]
        [AttributeDataMember("Llave", SQLTypeBasic.TEXT)]
        public string Llave { get; set; }

        //Carga de objetivos
        [DataMember]
        [AttributeDataMember("StartDateObj", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long StartDateObj { get; set; }

        [Display(Name = "Inicio carga de objetivos")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        //[Required(ErrorMessage = "Inicio carga de objetivos requerida")]
        public string StartDateObjTemp { get; set; }

        [DataMember]
        [AttributeDataMember("FinishDateObj", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FinishDateObj { get; set; }

        [Display(Name = "Fin carga de objetivos")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        //[Required(ErrorMessage = "Fin carga de objetivos requerida")]
        public string FinishDateObjTemp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activar notificaciones")]
        [Display(Name = "Activar notificaciones")]
        [AttributeDataMember("NotAct1", SQLTypeBasic.BLOB)]
        public bool NotAct1 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Hora de notificación es requerido")]
        [Display(Name = "Hora de notificación")]
        [StringLength(10, ErrorMessage = "El campo no puede tener más de 10 letras.")]
        [RegularExpression(Utilidades.pattTime, ErrorMessage = "El campo debe de tener el formato correcto")]
        [AttributeDataMember("HoraAct1", SQLTypeBasic.TEXT)]
        public string HoraAct1 { get; set; }
        //Carga de objetivos


        //evaluacion 1er periodo
        [DataMember]
        [AttributeDataMember("StartDateEva", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long StartDateEva { get; set; }

        [Display(Name = "Fecha de inicio evaluación")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string StartDateEvaTemp { get; set; }

        [DataMember]
        [AttributeDataMember("FinishDateEva", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FinishDateEva { get; set; }

        [Display(Name = "Fecha de fin evaluación")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string FinishDateEvaTemp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activar notificaciones")]
        [Display(Name = "Activar notificaciones")]
        [AttributeDataMember("NotAct2", SQLTypeBasic.BLOB)]
        public bool NotAct2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Hora de notificación es requerido")]
        [Display(Name = "Hora de notificación")]
        [StringLength(10, ErrorMessage = "El campo no puede tener más de 10 letras.")]
        [RegularExpression(Utilidades.pattTime, ErrorMessage = "El campo debe de tener el formato correcto")]
        [AttributeDataMember("HoraAct2", SQLTypeBasic.TEXT)]
        public string HoraAct2 { get; set; }

        //evaluacion 1er periodo


        //Calibracion 1er periodo
        [DataMember]
        [AttributeDataMember("StartDateCali", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long StartDateCali { get; set; }

        [Display(Name = "Fecha inicio de calibración 1er periodo")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string StartDateCaliTemp { get; set; }

        [DataMember]
        [AttributeDataMember("FinishDateCali", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FinishDateCali { get; set; }

        [Display(Name = "Fecha fin de calibración 1er periodo")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string FinishDateCaliTemp { get; set; }

 

        //Calibracion 1er periodo



        //evaluacion 2do periodo
        [DataMember]
        [AttributeDataMember("StartDateEva2", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long StartDateEva2 { get; set; }

        [Display(Name = "Inicio Evaluación Fin de año")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string StartDateEva2Temp { get; set; }

        [DataMember]
        [AttributeDataMember("FinishDateEva2", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FinishDateEva2 { get; set; }

        [Display(Name = "Fin Evaluación Fin de año")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string FinishDateEva2Temp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activar notificaciones")]
        [Display(Name = "Activar notificaciones")]
        [AttributeDataMember("NotAct3", SQLTypeBasic.BLOB)]
        public bool NotAct3 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Hora de notificación es requerido")]
        [Display(Name = "Hora de notificación")]
        [StringLength(10, ErrorMessage = "El campo no puede tener más de 10 letras.")]
        [RegularExpression(Utilidades.pattTime, ErrorMessage = "El campo debe de tener el formato correcto")]
        [AttributeDataMember("HoraAct3", SQLTypeBasic.TEXT)]
        public string HoraAct3 { get; set; }
        //evaluacion 2do periodo


        //Calibracion 2do periodo
        [DataMember]
        [AttributeDataMember("StartDateCali2", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long StartDateCali2 { get; set; }

        [Display(Name = "Fecha inicio de calibración 2do periodo")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string StartDateCali2Temp { get; set; }

        [DataMember]
        [AttributeDataMember("FinishDateCali2", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FinishDateCali2 { get; set; }

        [Display(Name = "Fecha fin de calibración 2do periodo")]
        //[RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        public string FinishDateCali2Temp { get; set; }
        //Calibracion 2do periodo


        [DataMember]
        [Display(Name = "País")]
        [AttributeDataMember("Pais", SQLTypeBasic.INTEGER)]
        public int Country { get; set; }

        public string PaisTempral { get; set; }
        public string Grupo { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activo")]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }

        [DataMember]
        [Display(Name = "Etapa")]
        [AttributeDataMember("Etapa", SQLTypeBasic.INTEGER)]
        public int Etapa { get; set; }
        public string EtapaDsc { get; set; }
        //0-Carga de objetivos
        //1-Periodo evaluación 1er periodo
        //2-Calibracion 1er periodo
        //3-Periodo evaluación 2do periodo
        //4-Calibracion 2do periodo
        //5-Termino evaluacion

        public string porcentaje{ get; set; }
        [DataMember]
        [AttributeDataMember("Modificadopor", SQLTypeBasic.TEXT)]
        public string Modificadopor { get; set; }
        public int Status { get; set; }
    }
}
