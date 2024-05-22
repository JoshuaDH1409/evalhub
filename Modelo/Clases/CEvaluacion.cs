using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modelo.Clases
{
    public class CEvaluacion
    {
    public EPeriodos periodo { get; set;}

    public CSessionEval sesion { get; set; }
    
    public ELogin Jefedir { get; set; }

    public ELogin JefeL2 { get; set; }

    public List<EObjetives> Liobjetivos { get; set;}

    public List<ECompetemces> ListaCompetencias { get; set; }
    public List<EEscaleta> Escaleta { get; set; }


     public List<EPersonalDP> ListaPersonalPDP { get; set; }
     public List<EPersonalTP> ListaPersonalPTP { get; set; }

        public bool EvalL2 { get; set; }
        public CEvaluacion()
        {
            Liobjetivos = new List<EObjetives>();
        }
    }


    
}
