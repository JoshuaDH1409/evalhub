using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlantillaMVC.Models
{
    public class RecuperarContraseña
    {

        [Required(ErrorMessage = "Correo electrónico  requerido")]
        [Display(Name = "Correo electrónico ")]
        [RegularExpression(@"^[A-Za-z0-9._-]+@(?:[A-Za-z0-9])+(\.[A-Za-z]{2,})+$", ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(100, ErrorMessage = "El campo no puede tener más de 100 letras.")]       
        public string Email { get; set; }
    }
    
}
