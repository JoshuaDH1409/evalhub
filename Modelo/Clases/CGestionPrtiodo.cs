using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modelo.Clases
{
    public class CPeriodo
    {
    public EPeriodos periodo { get; set;}

    public List<CSessionEval> listSesion { get; set; }

    }

    public class CPeriodoUserKPI
    {
        public ELogin Login { get; set; }
        public List<EObjetivoPais> ObjetivoPais { get; set; }
        public List<EObjetives> Objetives { get; set; }
        public List<EEscaleta> Escaleta { get; set; }
        public EPeriodos Periodo { get; set; }
        public Results Results { get; set; }
    }
    public class Results
    {
        public int CompB_SumaPonderados { get; set; }
        public int CompB_SumaCumplimiento { get; set; }
        public decimal CompB_SumaValor { get; set; }
        public decimal CompB_Score { get; set; }
        public int CompB_Weighting { get; set; }
        public decimal CompB_FinalScore { get; set; }
        public int CompC_SumaPonderados { get; set; }
        public int CompC_SumaCumplimiento { get; set; }
        public decimal CompC_SumaValor { get; set; }
        public int CompC_Score { get; set; }
        public int CompC_Weighting { get; set; }
        public decimal CompC_FinalScore { get; set; }
        public int Total_Weighting { get; set; }
        public decimal Total_Score { get; set; }
    }
}
