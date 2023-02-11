using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlantillaMVC.Models
{
    public class CambioContraseña
    {

        [Required(ErrorMessage = "Nueva contraseña es requerida")]
        [Display(Name = "Nueva ")]
        [RegularExpression("^[A-Z0-9a-z]*$", ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(100, ErrorMessage = "El campo no puede tener más de 100 letras.")]       
        public string Ncontrasena { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña es requerida")]
        [Display(Name = "Confirmar ")]
        [RegularExpression("^[A-Z0-9a-z]*$", ErrorMessage = "El campo debe de tener el formato correcto")]
        [StringLength(100, ErrorMessage = "El campo no puede tener más de 100 letras.")]
        public string ConContrasena { get; set; }
    }
    
}
