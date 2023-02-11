namespace Modelo.Clases
{
    public class CSession
    {
        public ELogin Login { get; set; }
        public EPais Pais { get; set; }
        public EPerfil Perfil { get; set; }
        public ELogin Evaluador { get; set; }
        public EEval Evaluacion { get; set; }
        public EPeriodos PeriodoActual { get; set; }

    }


}
