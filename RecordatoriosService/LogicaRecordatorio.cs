using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo;
using CapaLogica;

namespace RecordatoriosService
{
    class LogicaRecordatorio
    {
        internal static readonly CapaLogica.Interface.Interface1 negocio = new CapaLogica.CapaLogica();

        public void RecordatorioPeriodos()
        {
            List<EPeriodos> ListaPerioidos = negocio.RecuperaPeriodos();
            ListaPerioidos = (from item in ListaPerioidos
                              where item.Activo
                              select item).ToList();

            if (ListaPerioidos != null)
            {
                foreach (EPeriodos Periodo in ListaPerioidos)
                {
                    if (Periodo.Status == 0)
                    {
                        if (Periodo.StartDateObj < DateTime.Now.Ticks && Periodo.FinishDateObj > DateTime.Now.Ticks && Periodo.NotAct1)
                        {
                            string Hora = DateTime.Now.ToString("hh");
                            string HoraPeriodo = Periodo.HoraAct1.Substring(2);
                            Bitacora.NuevaEntrada("Comparacion hora Actual->" + Hora + " hora Periodo->" + HoraPeriodo, "RecordatorioPeriodos");
                            if (Hora == HoraPeriodo)
                            {
                                Bitacora.NuevaEntrada("Inicio Recordatorios pendiente objetivos periodo ->" + Periodo.id, "RecordatorioPeriodos");
                                negocio.EjecutarRecordatorios(Periodo.id);
                            }
                        }
                        else if (Periodo.Status == 1)
                        {
                            if (Periodo.StartDateEva < DateTime.Now.Ticks && Periodo.FinishDateEva > DateTime.Now.Ticks && Periodo.NotAct2)
                            {
                                string Hora = DateTime.Now.ToString("hh");
                                string HoraPeriodo = Periodo.HoraAct2.Substring(2);
                                Bitacora.NuevaEntrada("Comparacion hora Actual->" + Hora + " hora Periodo->" + HoraPeriodo, "RecordatorioPeriodos");
                                if (Hora == HoraPeriodo)
                                {
                                    Bitacora.NuevaEntrada("Inicio Recordatorios pendiente evaluacion medio año periodo ->" + Periodo.id, "RecordatorioPeriodos");
                                    negocio.EjecutarRecordatorios(Periodo.id);
                                }
                            }
                        }
                        else if (Periodo.Status == 3)
                        {
                            if (Periodo.StartDateEva2 < DateTime.Now.Ticks && Periodo.FinishDateEva2 > DateTime.Now.Ticks && Periodo.NotAct3)
                            {
                                string Hora = DateTime.Now.ToString("hh");
                                string HoraPeriodo = Periodo.HoraAct3.Substring(2);
                                Bitacora.NuevaEntrada("Comparacion hora Actual->" + Hora + " hora Periodo->" + HoraPeriodo, "RecordatorioPeriodos");
                                if (Hora == HoraPeriodo)
                                {
                                    Bitacora.NuevaEntrada("Inicio Recordatorios pendiente evaluacion fin de año periodo ->" + Periodo.id, "RecordatorioPeriodos");
                                    negocio.EjecutarRecordatorios(Periodo.id);
                                }
                            }
                        }

                    }
                }
            }


        }
    }
}
