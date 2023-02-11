using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceEvalDes
{
    public partial class Service1 : ServiceBase
    {
        internal static readonly CapaLogica.Interface.Interface1 negocio = new CapaLogica.CapaLogica();
        private Timer timer1 = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 40000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.Timer1_Tick);
            timer1.Enabled = true;
            PlantillaMVC.Bitacora.NuevaEntrada("inicio del servicio de windows", "OnStart");
        }

        private void Timer1_Tick(object sender, ElapsedEventArgs e)
        {
            string tem = DateTime.Now.ToShortTimeString();
            PlantillaMVC.Bitacora.NuevaEntrada(tem, "timer tick");
            try
            {
                if (tem == "10:10 a.m.")
                {
                    Procesar();
                }
                else
                {
                    PlantillaMVC.Bitacora.NuevaEntrada("Se termino el proceso" + DateTime.Now.ToShortTimeString(), "timer tick");
                }
            }
            catch (Exception ex)
            {
                PlantillaMVC.Bitacora.NuevaEntrada("Error->" + ex.ToString(), "ERROR");
            }
        }

        public static void Procesar()
        {
          List<Modelo.EPeriodos> ListaPer = negocio.RecuperaPeriodos();
            foreach (Modelo.EPeriodos periodo in ListaPer)
            {
                if (periodo.Activo)
                {
                    PlantillaMVC.Bitacora.NuevaEntrada("Se termino el proceso" + DateTime.Now.ToShortTimeString(), "timer tick");

                }
            }
        }

        protected override void OnStop()
        {
            PlantillaMVC.Bitacora.NuevaEntrada("Se termino el proceso"+DateTime.Now.ToShortTimeString(), "timer tick");
        }
    }
}
