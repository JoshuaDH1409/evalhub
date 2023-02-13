using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlantillaMVC.Models
{
    public class EFlitrosDash
    {

        [Display(Name = "País")]
        [Required(ErrorMessage = "País requerido")]
        public List<int> Pais { get; set; }

        [Display(Name = "Periodo")]
        [Required(ErrorMessage = "Periodo requerido")]
        public int Periodo { get; set; }

        [Display(Name = "Área")]
        public string Area { get; set; }

        [Display(Name = "División")]
        public string Division { get; set; }

        [Display(Name = "Etapas")]
        public bool PeriodoActivo { get; set; }



    }

}
