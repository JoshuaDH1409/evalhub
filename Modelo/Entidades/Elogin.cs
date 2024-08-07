using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using General;
using Modelo.Clases;
using System;

namespace Modelo
{
    [AttributeDataClass("TBL_usuario")]
    [DataContract]
    public class ELogin : EntidadBase
    {
        [DataMember]
        [AttributeDataMember("id", SQLTypeBasic.INTEGER)]
        public int id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "ID SAP es requerido")]
        [Display(Name = "ID SAP")]
        [RegularExpression(Utilidades.PattNumero, ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("id_sap", SQLTypeBasic.TEXT)]
        public string id_sap { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Nombre requerido")]
        [Display(Name = "Nombre(s)")]
        [RegularExpression(Utilidades.PattTextoValido, ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("Nombre", SQLTypeBasic.TEXT)]
        public string Nombre { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Apellido paterno requerido")]
        [Display(Name = "Apellido Paterno")]
        [RegularExpression(Utilidades.PattTextoValido, ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("ApellidoPat", SQLTypeBasic.TEXT)]
        public string ApellidoPat { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Apellido materno requerido")]
        [Display(Name = "Apellido Materno")]
        [RegularExpression(Utilidades.PattTextoValido, ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("ApellidoMat", SQLTypeBasic.TEXT)]
        public string ApellidoMat { get; set; }

        public bool NoAplica { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Puesto requerido")]
        [Display(Name = "Puesto")]
        [StringLength(150, ErrorMessage = "El campo no puede tener más de 150 letras.")]
        [AttributeDataMember("Puesto", SQLTypeBasic.TEXT)]
        public string Puesto { get; set; }

        [DataMember]
        [Required(ErrorMessage = "División requerida")]
        [Display(Name = "División")]
        [AttributeDataMember("Division", SQLTypeBasic.TEXT)]
        public string Division { get; set; }

        [DataMember]
        [AttributeDataMember("FechaIngreso", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FechaIngreso { get; set; }


        [Display(Name = "Fecha de ingreso")]
        [RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        [Required(ErrorMessage = "Fecha de ingreso requerida")]
        public string FechaIngresoTemp {
            get
            {
                if(FechaIngreso!=0)
                return new Utils().TiksToDate(FechaIngreso);
                else
                    return new Utils().TiksToDate(DateTime.Today.Ticks);
            }
        }

        [DataMember]
        [AttributeDataMember("FechaAntiguedad", SQLTypeBasic.NUMERIC, AceptaNulos = true, DatosParaNulos = new object[] { 0 })]
        public long FechaAntiguedad { get; set; }


        [Display(Name = "Fecha de antigüedad")]
        [RegularExpression(Utilidades.pattDate, ErrorMessage = "verifique el formato del campo")]
        //[Required(ErrorMessage = "Fecha de antigüedad requerida")]
        public string FechaAntiguedadTemp { get;set;
        }

        [DataMember]
        [Required(ErrorMessage = "Jefe inmediato requerido")]
        [Display(Name = "Jefe Inmediato")]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("EvaluadorIdSap", SQLTypeBasic.TEXT)]
        public string EvaluadorIdSap { get; set; }

        [Display(Name = "Jefe Inmediato")]
        [Required(ErrorMessage = "Jefe inmediato requerido")]
        public string EvaluadorNombre { get; set; }

        [DataMember]
        [Required(ErrorMessage = "País es requerido")]
        [Display(Name = "País")]
        [AttributeDataMember("Pais", SQLTypeBasic.INTEGER)]
        public int Pais { get; set; }
        public EPais PaisM { get; set; }
        public string PaisTemp { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Perfil es requerido")]
        [Display(Name = "Perfil")]
        [AttributeDataMember("perfil", SQLTypeBasic.INTEGER)]
        public int perfil { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Activo")]
        [Display(Name = "Activo")]
        [AttributeDataMember("Activo", SQLTypeBasic.BLOB)]
        public bool Activo { get; set; }

        [DataMember]
        [Display(Name = "Calibración")]
        [AttributeDataMember("calibracion", SQLTypeBasic.BLOB)]
        public bool calibracion { get; set; }

        [DataMember]
        [Display(Name = "Bono Anual")]
        [AttributeDataMember("bonoAnual", SQLTypeBasic.BLOB)]
        public bool bonoAnual { get; set; }

        [DataMember]
        [Display(Name = "PaisesRegion")]
        [AttributeDataMember("paisesRegion", SQLTypeBasic.TEXT)]
        public string paisesRegion { get; set; }


        [DataMember]
        [Required(ErrorMessage = "Correo electrónico  requerido")]
        [Display(Name = "Correo electrónico")]
        [RegularExpression(Utilidades.PattEmail, ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(100, ErrorMessage = "El campo no puede tener más de 100 letras.")]
        [AttributeDataMember("Email", SQLTypeBasic.TEXT)]
        public string Email { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Contraseña requerido")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "El campo no puede tener más de 50 letras.")]
        [AttributeDataMember("password", SQLTypeBasic.TEXT)]
        public string Password { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Notificar")]
        [Display(Name = "Notificar")]
        [AttributeDataMember("Notificar", SQLTypeBasic.BLOB)]
        public bool Notificar { get; set; }
        public string NombreCompleto
        {
            get
            {
                return Nombre + " " + ApellidoPat + " " + ApellidoMat;
            }
        }

        [DataMember]
        [AttributeDataMember("Modificadopor", SQLTypeBasic.TEXT)]
        public string Modificadopor { get; set; }
    }
}
