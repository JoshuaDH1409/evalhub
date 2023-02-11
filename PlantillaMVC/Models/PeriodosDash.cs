using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlantillaMVC.Models
{
    public class EPeriodosDash
    {
        [Required(ErrorMessage = "Tipo de periodo es requerido")]
        [Display(Name = "Periodo")]
        public int Periodo { get; set; }

        [Display(Name = "País")]
        public int pais { get; set; }

    }

}
