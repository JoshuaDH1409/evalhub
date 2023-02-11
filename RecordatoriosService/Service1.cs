using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Modelo;
using CapaLogica;


namespace RecordatoriosService
{
    public partial class Service1 : ServiceBase
    {
        //internal static  CapaLogica.Interface.Interface1 negocio = new CapaLogica.CapaLogica();
        public Service1()
        {
            InitializeComponent();
        }

        //Timer para el control del tiempo entre llamadas.
        private static System.Timers.Timer myTimer = new System.Timers.Timer();
      
        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry("In OnStart" + DateTime.Now.ToString());       
            Bitacora.NuevaEntrada("Inicio del servicio", "OnStart");        
            //Intervalo de tiempo entre llamadas.
            myTimer.Interval = 2400000;
            //Evento a ejecutar cuando se cumple el tiempo.
            Bitacora.NuevaEntrada("inicio del contador", "myTimer_Elapsed");
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
            Bitacora.NuevaEntrada("fin de contador", "myTimer_Elapsed");
            myTimer.AutoReset = true;
           //Habilitar el Timer.
            myTimer.Enabled = true;
        }

        void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            myTimer.Enabled = false;
            this.EventLog.WriteEntry("Entra al timer" + DateTime.Now.ToString());
            Bitacora.NuevaEntrada("despues del constructor", "myTimer_Elapsed");
            RecordatorioPeriodos(); 
            Bitacora.NuevaEntrada("Renicio del contador", "myTimer_Elapsed");
            this.EventLog.WriteEntry("Sale del timer" + DateTime.Now.ToString());
            myTimer.Enabled = true;
        }

        protected override void OnStop()
        {
          myTimer.Stop();
          myTimer.Dispose();
          this.EventLog.WriteEntry("Se terminó el servicio de recordatorios" + DateTime.Now.ToString());
          Bitacora.NuevaEntrada("Fin del servicio", "OnStop");
        }

        public void RecordatorioPeriodos()
        {

            this.EventLog.WriteEntry("Entra a recordatoriosperiodos" + DateTime.Now.ToString());
            try
            {
                CapaLogica.Interface.Interface1 negocio = new CapaLogica.CapaLogica();
                List<EPeriodos> ListaPerioidos = negocio.RecuperaPeriodos();

                this.EventLog.WriteEntry("****Recupera lista de Periodos num periodos" + ListaPerioidos.Count.ToString());
                Bitacora.NuevaEntrada("****Recupera lista de Periodos num periodos" + ListaPerioidos.Count.ToString(), "RecordatorioPeriodos");

                if (ListaPerioidos != null)
                {
                    ListaPerioidos = (from item in ListaPerioidos
                                      where item.Activo
                                      select item).ToList();
                   
                    foreach (EPeriodos Periodo in ListaPerioidos)
                    {
                            this.EventLog.WriteEntry("++++Status Periodo->" + Periodo.Status.ToString());
                            Bitacora.NuevaEntrada("++++Status Periodo->" + Periodo.Status.ToString(), "RecordatorioPeriodos");

                        if (Periodo.Status == 0)
                        {
                            this.EventLog.WriteEntry("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString());
                            Bitacora.NuevaEntrada("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString(), "RecordatorioPeriodos");
                            if (Periodo.StartDateObj < DateTime.Now.Ticks && Periodo.FinishDateObj > DateTime.Now.Ticks && Periodo.NotAct1)
                            {
                                string Hora = DateTime.Now.ToString("HH");
                                string HoraPeriodo = Periodo.HoraAct1.Substring(0, 2);
                                Bitacora.NuevaEntrada("Comparacion hora Actual->" + Hora + " hora Periodo->" + HoraPeriodo, "RecordatorioPeriodos");
                                if (Hora == HoraPeriodo)
                                {
                                    Bitacora.NuevaEntrada("Inicio Recordatorios pendiente objetivos periodo ->" + Periodo.id, "RecordatorioPeriodos");
                                    negocio.EjecutarRecordatorios(Periodo.id);
                                }
                            }
                        }
                        else if (Periodo.Status == 1)
                        {
                            this.EventLog.WriteEntry("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString());
                            Bitacora.NuevaEntrada("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString(), "RecordatorioPeriodos");
                            if (Periodo.StartDateEva < DateTime.Now.Ticks && Periodo.FinishDateEva > DateTime.Now.Ticks && Periodo.NotAct2)
                            {
                                string Hora = DateTime.Now.ToString("HH");
                                string HoraPeriodo = Periodo.HoraAct2.Substring(0, 2);
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
                            this.EventLog.WriteEntry("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString());
                            Bitacora.NuevaEntrada("Fecha inicio->" + Periodo.StartDateObj.ToString() + " Fecha Fin->" + Periodo.FinishDateObj.ToString() + " Fecha actual->" + DateTime.Now.Ticks.ToString(), "RecordatorioPeriodos");
                            if (Periodo.StartDateEva2 < DateTime.Now.Ticks && Periodo.FinishDateEva2 > DateTime.Now.Ticks && Periodo.NotAct3)
                            {
                                string Hora = DateTime.Now.ToString("HH");
                                string HoraPeriodo = Periodo.HoraAct3.Substring(0, 2);
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
            catch (Exception ex)
            {
                this.EventLog.WriteEntry("#### Error->" + DateTime.Now.ToString() + ex.ToString());
                Bitacora.NuevaEntrada("#### Error->" + DateTime.Now.ToString() + ex.ToString(), "RecordatorioPeriodos");
            }
        }




    }
}
