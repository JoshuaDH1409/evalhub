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

namespace CapaLogica.Evaluaciones
{
    class ClsEvaluacion
    {
        public static EStatus RecuperaStatusEval(int id)
        {
            try
            {
                List<EStatus> LiPeriodo = AccesoDB.Read(new EStatus(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EStatus).GetProperty("Id"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)
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
        public static List<EEval> RecuperaEvaluaciones()
        {
            try
            {
                return AccesoDB.ReadAll(new EEval());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static List<EEval> RecuperaEvaluacionesPeriodo(int Periodo)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                   new Cliterio(typeof(EEval).GetProperty("periodo"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND,typeof(EEval).GetProperty("Activo"), OperadoresRelacionales.IGUAL,true, TipoValor.Boleano)
                });

                if (liEval.Count > 0)
                    return liEval;
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

        public static List<EEval> RecuperaEvaluacionesPeriodoTotal(int Periodo)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                   new Cliterio(typeof(EEval).GetProperty("periodo"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero)
                });

                if (liEval.Count > 0)
                    return liEval;
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

        public static List<EEval> RecuperaEvaluacionesActivas()
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("Activo"), OperadoresRelacionales.IGUAL,true, TipoValor.Boleano)
                });

                if (liEval.Count > 0)
                    return liEval;
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

        public static List<Modelo.Clases.CSessionEval> RecuperaEvaluacionesPeriodoUsuarios(int Periodo)
        {
            List<Modelo.Clases.CSessionEval> Respuesta = new List<Modelo.Clases.CSessionEval>();
            //List<EEval> Evaluaciones = RecuperaEvaluacionesPeriodo(Periodo);
            List<EEval> Evaluaciones = RecuperaEvaluacionesPeriodoTotal(Periodo);

            try
            {
                foreach (EEval item in Evaluaciones)
                {
                    Modelo.Clases.CSessionEval Temporal = new Modelo.Clases.CSessionEval();

                    Temporal.Evaluacion = item;
                    Temporal.Login = Seguridad.Seguridad.RecuperaUnUsuario(item.id_usuario);
                    //Temporal.Login.Division = Seguridad.Seguridad.RecuperaDivisionPersona(Temporal.Login.Division);
                    Respuesta.Add(Temporal);
                }
                if (Respuesta.Count > 0)
                {
                    return Respuesta;
                }
                else
                {
                    return null;
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

        public static EEval RecuperaUnaEaluacion(int id)
        {
            try
            {
                List<EEval> liCorreos = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("id"), OperadoresRelacionales.IGUAL,id, TipoValor.Numero)
                });

                if (liCorreos.Count > 0)
                    return liCorreos[0];
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

        public static EEval RecuperaEvaluacionActivaUsario(int idUser)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("id_usuario"), OperadoresRelacionales.IGUAL,idUser, TipoValor.Numero),
                    new Cliterio(OperadoresLogicos.AND,typeof(EEval).GetProperty("Activo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                if (liEval.Count > 0)
                {
                    liEval[0].StatusDsc = RecuperaStatusEval(liEval[0].Status).Name;
                    return liEval[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                return null;    
            }
        }
        public static EEval RecuperaEvaluacionPeriodoUsario(int idUser, int periodo)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("id_usuario"), OperadoresRelacionales.IGUAL,idUser, TipoValor.Numero),
                    new Cliterio(OperadoresLogicos.AND,typeof(EEval).GetProperty("periodo"), OperadoresRelacionales.IGUAL, periodo, TipoValor.Numero)
                });

                if (liEval.Count > 0)
                {
                    liEval[0].StatusDsc = RecuperaStatusEval(liEval[0].Status).Name;
                    return liEval[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                return null;
            }
        }

        public static bool GuardaEvaluacion(EEval Evaluacion)
        {
            try
            {


                if (Evaluacion.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Evaluacion.Operacion = TipoOperacion.Modificar;
                        OperacionEvauacion(Evaluacion, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        Evaluacion.Operacion = TipoOperacion.Nuevo;
                        //periodo.Status = 0;

                        object tmp = AccesoDB.MaxId(tran, new EEval(), typeof(EEval).GetProperty("id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        Evaluacion.id = max;
                        OperacionEvauacion(Evaluacion, tran);
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

        private static void OperacionEvauacion(EEval Evaluacion, ITransactionCRUD tran)
        {
            try
            {
                switch (Evaluacion.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        //object tmp = AccesoDB.MaxId(tran, Usuario, typeof(ELogin).GetProperty("id"));
                        //if (DBNull.Value.Equals(tmp))
                        //    tmp = 0;
                        //Usuario.UsuarioID = (int)tmp + 1;
                        AccesoDB.Save(tran, Evaluacion);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, Evaluacion, new List<Cliterio>()
                        {
                            new Cliterio(typeof(ELogin).GetProperty("id"), OperadoresRelacionales.IGUAL, Evaluacion.id, TipoValor.Numero)
                        });
                        break;
                    case TipoOperacion.Borrar:
                        AccesoDB.delete(tran, Evaluacion);
                        break;
                    case TipoOperacion.BajaLogica:
                        break;
                    default:
                        throw new Exception("No se encontro la operación");
                }

                Evaluacion.Operacion = TipoOperacion.Lectura;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Persona: {0}", Evaluacion)});
            }
        }


        public static List<Modelo.Clases.CHistorial> RecuperaHistorialUsr(int idUsr)
        {

            List<Modelo.Clases.CHistorial> ListResultado = new List<Modelo.Clases.CHistorial>();
            List<EEval> Lieval = RecuperaListaEvaluacionesPasadasUsario(idUsr);

            if (Lieval != null)
            {
                foreach (EEval item in Lieval)
                {
                    Modelo.Clases.CHistorial aux = new Modelo.Clases.CHistorial();
                    EPeriodos temp = Periodo.ClsPeriodo.RecuperaUnPeriodo(item.periodo);
                    aux.Periodo = temp;
                    aux.Evaluacion = item;
                    ListResultado.Add(aux);
                }
            }
            return ListResultado;
        }


        public static List<EEval> RecuperaListaEvaluacionesPasadasUsario(int idUser)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("id_usuario"), OperadoresRelacionales.IGUAL,idUser, TipoValor.Numero),
                    new Cliterio(OperadoresLogicos.AND,typeof(EEval).GetProperty("Activo"), OperadoresRelacionales.IGUAL, false, TipoValor.Boleano)
                });

                if (liEval.Count > 0)
                    return liEval;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                return null;
            }
        }

        public static EEval RecuperaEvaluacionIdHistorial(int idEval)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                    new Cliterio(typeof(EEval).GetProperty("id"), OperadoresRelacionales.IGUAL,idEval, TipoValor.Numero)
                });

                if (liEval.Count > 0)
                    return liEval[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});

                return null;
            }
        }

        public static List<EEval> RecuperaEvaluacionesPeriodoStatus(int Periodo, int status)
        {
            try
            {
                List<EEval> liEval = AccesoDB.Read(new EEval(), new List<Cliterio>()
                {
                   new Cliterio(typeof(EEval).GetProperty("periodo"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(EEval).GetProperty("Status"), OperadoresRelacionales.IGUAL, status, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND,typeof(EEval).GetProperty("Activo"), OperadoresRelacionales.IGUAL,true, TipoValor.Boleano)
                });

                if (liEval.Count > 0)
                    return liEval;
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

        //Nandarek -- generando metodos para la vista de evaluación
        public static List<VEvaluacion> GetEvalView()
        {
            try
            {
                return AccesoDB.ReadAll(new VEvaluacion());
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static List<VEvaluacion> GetEvalViewByPeriodActive()
        {
            try
            {
                List<VEvaluacion> listEval = AccesoDB.Read(new VEvaluacion(), new List<Cliterio>()
                {
                   new Cliterio(typeof(VEvaluacion).GetProperty("PeriodoActivo"), OperadoresRelacionales.IGUAL, 1, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvaluacion).GetProperty("UsuarioActivo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                return listEval;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static List<VEvaluacion> GetEvalViewByPeriodCountry(int Periodo, int Pais)
        {
            try
            {
                List<VEvaluacion> listEval = AccesoDB.Read(new VEvaluacion(), new List<Cliterio>()
                {
                   new Cliterio(typeof(VEvaluacion).GetProperty("Periodo"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvaluacion).GetProperty("Pais"), OperadoresRelacionales.IGUAL, Pais, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvaluacion).GetProperty("UsuarioActivo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                return listEval;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }

        public static List<VEvaluacion> GetEvalViewByPeriodoEvaluator(int Periodo,string IdSap)
        {
            try
            {
                List<VEvaluacion> listEval = AccesoDB.Read(new VEvaluacion(), new List<Cliterio>()
                {
                   new Cliterio(typeof(VEvaluacion).GetProperty("EvaluadorSap"), OperadoresRelacionales.IGUAL,IdSap, TipoValor.Texto),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvaluacion).GetProperty("Periodo"), OperadoresRelacionales.IGUAL, Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvaluacion).GetProperty("UsuarioActivo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                return listEval;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }

        public static List<VEvalCompetences> GetEvalCompViewByPeriodCountry(int Periodo, int Pais)
        {
            try
            {
                List<VEvalCompetences> listEval = AccesoDB.Read(new VEvalCompetences(), new List<Cliterio>()
                {
                   new Cliterio(typeof(VEvalCompetences).GetProperty("PeriodoId"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvalCompetences).GetProperty("PaisId"), OperadoresRelacionales.IGUAL, Pais, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvalCompetences).GetProperty("UsuarioActivo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                return listEval;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        /*------------------- Objetivos ------------------------------------------------------------------------------------------------------------*/
        public static List<VEvalObj> GetEvalObjViewByPeriodCountryDiv(int Periodo, int Pais, string Division)
        {
            try
            {
                List<VEvalObj> listEval = AccesoDB.Read(new VEvalObj(), new List<Cliterio>()
                {
                   new Cliterio(typeof(VEvalObj).GetProperty("PeriodId"), OperadoresRelacionales.IGUAL,Periodo, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvalObj).GetProperty("CountryId"), OperadoresRelacionales.IGUAL, Pais, TipoValor.Numero),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvalObj).GetProperty("Division"), OperadoresRelacionales.IGUAL, Division, TipoValor.Texto),
                   new Cliterio(OperadoresLogicos.AND, typeof(VEvalObj).GetProperty("Activo"), OperadoresRelacionales.IGUAL, true, TipoValor.Boleano)
                });

                return listEval;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                          string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        /*_____________________________________________________________________________________________________________________________________________*/
    }
}
