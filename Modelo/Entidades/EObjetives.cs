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
    [AttributeDataClass("TBL_objetives")]
    [DataContract]
    public class EObjetives : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        public int idEval { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Titulo es requerido")]
        [Display(Name = "Objetivo")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("titulo", SQLTypeBasic.TEXT)]
        public string titulo { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Resultado es requerido")]
        [Display(Name = "Comentarios")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("resultado", SQLTypeBasic.TEXT)]
        public string resultado { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Auto evaluación es requerida")]
        [Display(Name = "Auto evaluación")]
        [AttributeDataMember("autoeval", SQLTypeBasic.INTEGER)]
        public int autoeval { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Calificación es requerida")]
        [Display(Name = "Calificación ")]
        [AttributeDataMember("calif", SQLTypeBasic.INTEGER)]
        public int calif { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Calificación final es requerido")]
        [Display(Name = "Calificación final")]
        [AttributeDataMember("califFinal", SQLTypeBasic.INTEGER)]
        public int califFinal { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Descripción requerida")]
        [Display(Name = "Descripción")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objdesc", SQLTypeBasic.TEXT)]   
        public string objdesc { get; set; }

        [DataMember]
        [Required(ErrorMessage = "objActs requerida")]
        [Display(Name = "Acciones")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objActs", SQLTypeBasic.TEXT)]
        public string objActs { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario jefe evaluación final requerido")]
        [Display(Name = "Comentario jefe evaluación final")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("comentariosJefeEvalFinal", SQLTypeBasic.TEXT)]
        public string comentariosJefeEvalFinal { get; set; }

        [DataMember]
        [Required(ErrorMessage = "ComentariosJefeEval requerida")]
        [Display(Name = "Comentarios")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComentariosJefeEval", SQLTypeBasic.TEXT)]
        public string ComentariosJefeEval { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Métrica (KPI) requerida")]
        [Display(Name = "Métrica (KPI)")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objMetricas", SQLTypeBasic.TEXT)]
        public string objMetricas { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Eval es requerido")]
        [Display(Name = "Calificación")]
        [AttributeDataMember("Eval", SQLTypeBasic.INTEGER)]
        public int Eval { get; set; }
        [DataMember]
        [Display(Name = "Peso ponderado %")]
        [AttributeDataMember("ponderado", SQLTypeBasic.INTEGER)]
        public int ponderado { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Resultado2 requerida")]
        [Display(Name = "Resultado")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("Resultado2", SQLTypeBasic.TEXT)]
        public string Resultado2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "AutoEval2 es requerido")]
        [Display(Name = "Auto evaluación")]
        [AttributeDataMember("AutoEval2", SQLTypeBasic.INTEGER)]
        public int AutoEval2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Calif2 es requerido")]
        [Display(Name = "Calificación")]
        [AttributeDataMember("Calif2", SQLTypeBasic.INTEGER)]
        public int Calif2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "CalifFinal2 es requerido")]
        [Display(Name = "Calificación acordada")]
        [AttributeDataMember("CalifFinal2", SQLTypeBasic.INTEGER)]
        public int CalifFinal2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Cumplimiento es requerido")]
        [Display(Name = "Cumplimiento %")]
        [AttributeDataMember("cumplimientoEvaluado", SQLTypeBasic.INTEGER)]
        public int cumplimientoEvaluado { get; set; }

        [DataMember]
        [Display(Name = "Comentarios")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComentariosJefeEvalFinall2", SQLTypeBasic.TEXT)]
        public string ComentariosJefeEvalFinall2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Cumplimiento es requerido")]
        [Display(Name = "Cumplimiento %")]
        [AttributeDataMember("cumplimientoEvaluador", SQLTypeBasic.INTEGER)]
        public int cumplimientoEvaluador { get; set; }

        [DataMember]
        [Display(Name = "Comentarios")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("ComentariosJefeEval2", SQLTypeBasic.TEXT)]
        public string ComentariosJefeEval2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Cumplimiento es requerido")]
        [Display(Name = "Cumplimiento %")]
        [AttributeDataMember("cumplimientoEvaluado2", SQLTypeBasic.INTEGER)]
        public int cumplimientoEvaluado2 { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Cumplimiento es requerido")]
        [Display(Name = "Cumplimiento %")]
        [AttributeDataMember("cumplimientoEvaluador2", SQLTypeBasic.INTEGER)]
        public int cumplimientoEvaluador2 { get; set; }

        [DataMember]
        [Display(Name = "Métrica (KPI)")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objMetricas2", SQLTypeBasic.TEXT)]
        public string objMetricas2 { get; set; }

        [DataMember]
        [Display(Name = "Descripción")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objDesc2", SQLTypeBasic.TEXT)]
        public string objDesc2 { get; set; }

        [DataMember]
        [Display(Name = "Acciones")]
        [StringLength(4999, ErrorMessage = "El campo no puede tener más de 5000 letras.")]
        [AttributeDataMember("objActs2", SQLTypeBasic.TEXT)]
        public string objActs2 { get; set; }

        [DataMember]
        [AttributeDataMember("etapa", SQLTypeBasic.INTEGER)]
        public int Etapa { get; set; }

        //etapa

        //inicio +++++++++++++++
        //0->Creado en un inicio

        //medio año +++++++++++
        //1->creado medio año usuario        #98FADB
        //2->creado medio jefe               #069B6C
        //3->editado por usuario             #A3BDFC
        //4->editado por jefe                #2E69FF
        //5->eliminado por usuario           #FF0000
        //medio año +++++++++++


        //sin Colores
        //Calif final +++++++++
        //6->creado por usuario
        //7->creado por jefe
        //8->editado por usuario
        //9->editado por jefe               
        //10->eliminado por usuario
        //Calif final +++++++++
        [DataMember]
        [AttributeDataMember("Modificadopor", SQLTypeBasic.TEXT)]
        public string Modificadopor { get; set; }

        [Display(Name = "Valor objetivo")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        public decimal ValorObj
        {
            get
            {
                if (cumplimientoEvaluador2 != 0)
                    return Decimal.Divide(ponderado * cumplimientoEvaluador2, 100);
                else
                    return 0;
            }
        }
        [Display(Name = "Valor objetivo(Mitad de Año)")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        public decimal ValorObj_MA
        {
            get
            {
                if (cumplimientoEvaluador != 0)
                    return Decimal.Divide(ponderado * cumplimientoEvaluador, 100);
                else
                    return 0;
            }
        }

    }
}
