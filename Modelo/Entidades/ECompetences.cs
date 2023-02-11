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
    [AttributeDataClass("TBL_competences")]
    [DataContract]
   public class ECompetemces : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "titulo requerido")]
        [Display(Name = "Título")]
        [AttributeDataMember("titulo", SQLTypeBasic.INTEGER)]
        public int titulo { get; set; }

        public string tituloTemp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "actividades requerido")]
        [Display(Name = "Actividades")]
        [StringLength(2999, ErrorMessage = "El campo no puede tener más de 3000 letras.")]
        [AttributeDataMember("actividades", SQLTypeBasic.TEXT)]
        public string actividades { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario requerido")]
        [Display(Name = "Comentario evaluado")]
        [StringLength(2999, ErrorMessage = "El campo no puede tener más de 3000 letras.")]
        [AttributeDataMember("resultados", SQLTypeBasic.TEXT)]
        public string resultados { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Comentario requerido")]
        [Display(Name = "Comentario evaluador")]
        [StringLength(2999, ErrorMessage = "El campo no puede tener más de 3000 letras.")]
        [AttributeDataMember("resultadosEvaluador", SQLTypeBasic.TEXT)]
        public string resultadosEvaluador { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Comentario final requerido")]
        [Display(Name = "Comentario evaluado")]
        [StringLength(2999, ErrorMessage = "El campo no puede tener más de 3000 letras.")]
        [AttributeDataMember("resultados2", SQLTypeBasic.TEXT)]
        public string resultados2 { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Comentario final requerido")]
        [Display(Name = "Comentario evaluador")]
        [StringLength(2999, ErrorMessage = "El campo no puede tener más de 3000 letras.")]
        [AttributeDataMember("resultados2Evaluador", SQLTypeBasic.TEXT)]
        public string resultados2Evaluador { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Eval requerido")]
        [AttributeDataMember("Eval", SQLTypeBasic.INTEGER)]
        public int Eval { get; set; }

        [DataMember]
        [Required(ErrorMessage = "etapa requerido")]
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
        //6->creado por usuario             #98FADB   
        //7->creado por jefe                #069B6C   
        //8->editado por usuario            #A3BDFC   
        //9->editado por jefe               #2E69FF   
        //10->eliminado por usuario         #FF0000
        //Calif final +++++++++
        [DataMember]
        [AttributeDataMember("Modificadopor", SQLTypeBasic.TEXT)]
        public string Modificadopor { get; set; }

    }
}
