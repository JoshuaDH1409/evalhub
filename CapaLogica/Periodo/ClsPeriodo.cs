using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo;
using CRUD.Transaction;
using CapaLogica.Funciones;
using System.Reflection;
using CRUD;
using General;
using CapaLogica.Correo;

namespace CapaLogica.Periodo
{
    class ClsPeriodo
    {
        public static List<EPeriodos> RecuperaPeriodos()
        {
            try
            {
                return AccesoDB.ReadAll(new EPeriodos());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static EPeriodos RecuperaUnPeriodo(int idPeriodo)
        {
            try
            {
                List<EPeriodos> LiPeriodo = AccesoDB.Read(new EPeriodos(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPeriodos).GetProperty("id"), OperadoresRelacionales.IGUAL,idPeriodo, TipoValor.Numero)

                });

                if (LiPeriodo.Count > 0)
                {
                    LiPeriodo[0].EtapaDsc = RecuperaEtapa(LiPeriodo[0].Etapa).EtapaDsc;
                    return LiPeriodo[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static List<EPeriodos> RecuperaUnPeriodosActivos()
        {
            try
            {
                List<EPeriodos> LiPeriodo = AccesoDB.Read(new EPeriodos(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPeriodos).GetProperty("Activo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)

                });

                if (LiPeriodo.Count > 0)
                    return LiPeriodo;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static List<EPeriodos> RecuperaTodosPeriodosPais(int Pais)
        {
            try
            {
                List<EPeriodos> LiPeriodo = AccesoDB.Read(new EPeriodos(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPeriodos).GetProperty("Country"), OperadoresRelacionales.IGUAL,Pais, TipoValor.Numero)

                });
                if (LiPeriodo.Count > 0)
                    return LiPeriodo;
                else
                    return new List<EPeriodos>();
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static Eetapa RecuperaEtapa(int id)
        {
            try
            {
                List<Eetapa> LiPeriodo = AccesoDB.Read(new Eetapa(), new List<Cliterio>()
                {
                    new Cliterio(typeof(Eetapa).GetProperty("Etapa"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)
                });
                if (LiPeriodo.Count > 0)
                {
                    return LiPeriodo[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static  EPeriodos RecuperaPeriodopais(int Pais)
        {
            try
            {
                List<EPeriodos> LiPeriodo = AccesoDB.Read(new EPeriodos(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EPeriodos).GetProperty("Country"), OperadoresRelacionales.IGUAL,Pais, TipoValor.Numero),
                    new Cliterio(OperadoresLogicos.AND,typeof(EPeriodos).GetProperty("Activo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)

                });
                if (LiPeriodo.Count > 0)
                {
                    LiPeriodo[0].EtapaDsc = RecuperaEtapa(LiPeriodo[0].Etapa).EtapaDsc;
                    return LiPeriodo[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static bool GuardaPeriodo(EPeriodos periodo)
        {
            try
            {
                if (periodo.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        periodo.Operacion = TipoOperacion.Modificar;
                        OperacionPeriodos(periodo, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        periodo.Operacion = TipoOperacion.Nuevo;
                        OperacionPeriodos(periodo, tran);
                        tran.Commit();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                throw;
            }

        }
        private static void OperacionPeriodos(EPeriodos periodo, ITransactionCRUD tran)
        {
            try
            {
                switch (periodo.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        object tmp = AccesoDB.MaxId(tran, new EPeriodos(), typeof(EPeriodos).GetProperty("id"));
                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;
                        int max = (int)tmp + 1;
                        periodo.id = max;
                        AccesoDB.Save(tran, periodo);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, periodo, new List<Cliterio>()
                        {
                            new Cliterio(typeof(EPeriodos).GetProperty("id"), OperadoresRelacionales.IGUAL, periodo.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        //AccesoDB.delete(tran, contrato);
                        break;
                    case TipoOperacion.BajaLogica:
                        break;
                    default:
                        throw new Exception("No se encontro la operación");
                }

                periodo.Operacion = TipoOperacion.Lectura;
            }

            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Periodo: {0}", periodo)});
            }
        }
        public void EjecutarRecordatorios(int IdPer)
        {
            EPeriodos Periodo = RecuperaUnPeriodo(IdPer);
            if (Periodo.Etapa == 0 && Periodo.Activo)
            {
                List<EEval> ListaPendientes = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 0);
                if (ListaPendientes != null)
                {
                    foreach (EEval item in ListaPendientes)
                    {
                        Notificar(item, 1);
                    }
                }

                List<EEval> ListaEval = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 1);
                if (ListaEval != null)
                {
                    foreach (EEval item in ListaEval)
                    {
                        Notificar(item, 2);
                    }
                }

            }
            else if (Periodo.Etapa == 1 && Periodo.Activo)
            {
                List<EEval> ListaPendientes = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 4);
                if (ListaPendientes != null)
                {
                    foreach (EEval item in ListaPendientes)
                    {
                        Notificar(item, 3);
                    }
                }

                List<EEval> ListaEval = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 5);
                if (ListaEval != null)
                {
                    foreach (EEval item in ListaEval)
                    {
                        Notificar(item, 4);
                    }
                }

            }
            else if (Periodo.Etapa == 3 && Periodo.Activo)
            {
                List<EEval> ListaPendientes = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 10);
                if (ListaPendientes != null)
                {
                    foreach (EEval item in ListaPendientes)
                    {
                        Notificar(item, 3);
                    }
                }

                List<EEval> ListaEval = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 11);
                if (ListaEval != null)
                {
                    foreach (EEval item in ListaEval)
                    {
                        Notificar(item, 4);
                    }
                }

                List<EEval> ListaEvalLv2 = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 12);
                if (ListaEval != null)
                {
                    foreach (EEval item in ListaEval)
                    {
                        Notificar(item, 5);
                    }
                }

            }
            //recordatorio Extemporaneos
            if (Periodo.Etapa <= 3)
            {
                List<EEval> ListaExtemporaneos = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 16);
                if (ListaExtemporaneos != null)
                {
                    foreach (EEval item in ListaExtemporaneos)
                    {
                        Notificar(item, 1);
                    }
                }

                List<EEval> ListaApro = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoStatus(IdPer, 17);
                if (ListaExtemporaneos != null)
                {
                    foreach (EEval item in ListaApro)
                    {
                        Notificar(item, 2);
                    }
                }
            }
        }
        private bool Notificar(EEval Evaluacion, int Recordatorio)
        {
            //1-Carga de objetivos-21
            //2-Pendiente de evaluación-22
            //3-Pendiente de auto Evaluación-23
            //4-pendiente de evaluar-24
            //5-Validar lvb2-25
            try
            {
                ELogin Usuario = Seguridad.Seguridad.RecuperaUnUsuario(Evaluacion.id_usuario);
                ELogin Jefe = Seguridad.Seguridad.RecuperaUnUsuarioSap(Evaluacion.id_evaluador);
                ELogin jefeLv2 = new ELogin();
                if (Recordatorio == 5)
                {
                    jefeLv2 = Seguridad.Seguridad.RecuperaUnUsuarioSap(Jefe.EvaluadorIdSap);
                }


                if (Recordatorio == 1 && Usuario.Notificar)
                {
                    ECorreos correo = Correo.ClsCorreos.RecuperaUnCorreo(21);//idCorreo
                    global::CapaLogica.Correo.EnvioCorreo MandarCorreo = new global::CapaLogica.Correo.EnvioCorreo();
                    MandarCorreo.SendMail("Soporte", Usuario.Email, " ", "Estimado:" + Usuario.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);

                }
                else if (Recordatorio == 2 && Jefe.Notificar)
                {
                    ECorreos correo = Correo.ClsCorreos.RecuperaUnCorreo(22);//idCorreo
                    global::CapaLogica.Correo.EnvioCorreo MandarCorreo = new global::CapaLogica.Correo.EnvioCorreo();
                    MandarCorreo.SendMail("Soporte", Jefe.Email, " ", "Estimado:" + Jefe.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                }
                else if (Recordatorio == 3 && Usuario.Notificar)
                {
                    ECorreos correo = Correo.ClsCorreos.RecuperaUnCorreo(23);//idCorreo
                    global::CapaLogica.Correo.EnvioCorreo MandarCorreo = new global::CapaLogica.Correo.EnvioCorreo();
                    MandarCorreo.SendMail("Soporte", Usuario.Email, " ", "Estimado:" + Usuario.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                }
                else if (Recordatorio == 4 && Jefe.Notificar)
                {
                    ECorreos correo = Correo.ClsCorreos.RecuperaUnCorreo(24);//idCorreo
                    global::CapaLogica.Correo.EnvioCorreo MandarCorreo = new global::CapaLogica.Correo.EnvioCorreo();
                    MandarCorreo.SendMail("Soporte", Jefe.Email, " ", "Estimado:" + Jefe.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                }
                else if (Recordatorio == 5 && jefeLv2 != null)
                {
                    if (jefeLv2.Notificar)
                    {
                        ECorreos correo = Correo.ClsCorreos.RecuperaUnCorreo(25);//idCorreo
                        global::CapaLogica.Correo.EnvioCorreo MandarCorreo = new global::CapaLogica.Correo.EnvioCorreo();
                        MandarCorreo.SendMail("Soporte", jefeLv2.Email, " ", "Estimado:" + jefeLv2.NombreCompleto + "<br><br><br>" + correo.Mensaje + " <br><br> *AUTOMATED SYSTEM MESSAGE - Please do not reply to this email*", correo.asunto);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});

                return false;
            }

        }

        // EN ClsPeriodo.cs


    }
}
