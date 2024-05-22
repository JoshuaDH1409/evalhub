using System;
using System.Collections.Generic;
using System.Linq;
using Modelo;
using CRUD.Transaction;
using CapaLogica.Funciones;
using CRUD;
using General;
using Modelo.Clases;

namespace CapaLogica.Seguridad
{
    class Seguridad
    {

        public static int ValidaAcceso(string usuario, string pass ) {

            ELogin entidad = RecuperaLoginPersona(usuario, pass);

            if (entidad != null && entidad.Activo)
            {
                return 1;
            }
            else {
                return 0;
            }

        }

        public static CSession obtenSession(string usuario, string pass) {
            CSession ResSession = new CSession();
            ResSession.Login = RecuperaLoginPersona(usuario, pass);
            ResSession.Pais = Pais.ClsPais.RecuperaUnPais(ResSession.Login.Pais);
            ResSession.Perfil = Perfil.PerfilCls.RecuperaUnPerfil(ResSession.Login.perfil);
            //ResSession.Login.DivisionM = RecuperaDivisionPersona(ResSession.Login.Division);
            //ResSession.Evaluador = RecuperaUnUsuarioSap(ResSession.Login.EvaluadorIdSap);
            //ResSession.Evaluacion = RecuperaUsuarioEval(ResSession.Login.id).Evaluacion;
            return ResSession;
        }
        
        private static ELogin RecuperaLoginPersona(string Email, string password)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("Email"), OperadoresRelacionales.IGUAL, Email, TipoValor.Texto),
                    new Cliterio(OperadoresLogicos.AND,typeof(ELogin).GetProperty("Password"), OperadoresRelacionales.IGUAL, password, TipoValor.Texto)
                });
                if (login.Count > 0)
                    return login[0];
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
        public static EDivision RecuperaDivisionPersona(int id)
        {
            try
            {
                List<EDivision> division = AccesoDB.Read(new EDivision(),
                    new List<Cliterio>() {
                        new Cliterio(typeof(EDivision).GetProperty("id"), OperadoresRelacionales.IGUAL, id, TipoValor.Numero) 
                    }
                );
                return division.First();
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }
        public static List<ELogin> RecuperaLogsIn()
        {
            try
            {
                List<ELogin> login = AccesoDB.ReadAll(new ELogin());

                if (login.Count > 0)
                    return login;
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

        public static ELogin RecuperaUnUsuarioSap(string id_sap)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("id_sap"), OperadoresRelacionales.IGUAL, id_sap, TipoValor.Texto)

                });

                if (login.Count > 0)
                {
                    //login[0].DivisionM = RecuperaDivisionPersona(login[0].Division);
                    return login[0];
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


        public static List<ELogin> RecuperaLogInsPais(int Pais)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("Pais"), OperadoresRelacionales.IGUAL, Pais, TipoValor.Numero)

                });

                if (login.Count > 0)
                {
                    //foreach (var l in login)
                    //{
                    //    l.DivisionM = RecuperaDivisionPersona(l.Division);
                    //}
                    return login;
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

        public static ELogin RecuperaUnUsuarioCorreo(string Correo)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("Email"), OperadoresRelacionales.IGUAL, Correo, TipoValor.Texto)

                });

                if (login.Count > 0)
                    return login[0];
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

        public static List<ELogin> RecuperaUsuariosPais(int pais)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("Pais"), OperadoresRelacionales.IGUAL, pais, TipoValor.Numero)

                });

                if (login.Count > 0)
                    return login;
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
        public static List<ELogin> RecuperaUsuariosTodos()
        {
            try
            {
                List<ELogin> login = AccesoDB.ReadAll(new ELogin());

                if (login.Count > 0)
                    return login;
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
        public static ELogin RecuperaUnUsuario(int id)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("id"), OperadoresRelacionales.IGUAL, id, TipoValor.Numero)

                });

                if (login.Count > 0)
                {
                    //login[0].DivisionM = RecuperaDivisionPersona(login[0].Division);
                    return login[0];
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

        public static List<ELogin> RecuperaSubSap(string JefeSap)
        {
            try
            {
                List<ELogin> login = AccesoDB.Read(new ELogin(), new List<Cliterio>()
                {
                    new Cliterio(typeof(ELogin).GetProperty("EvaluadorIdSap"), OperadoresRelacionales.IGUAL, JefeSap, TipoValor.Texto)

                });

                //if (login.Count > 0)
                    return login;
                //else
                //    return null;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                           new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
                throw;
            }
        }


        public static List<CSessionEval> RecuperaLiSubordinados(string JefeSap)
        {
            try
            {
                List<CSessionEval> respuesta = new List<CSessionEval>();
                List<ELogin> listaUsuarios = RecuperaSubSap(JefeSap);


                foreach (ELogin item in listaUsuarios)
                {
                    CSessionEval aux = new CSessionEval();
                    EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(item.perfil);
                    //item.DivisionM = Seguridad.RecuperaDivisionPersona(item.Division);
                    item.PaisM = Pais.ClsPais.RecuperaUnPais(item.Pais);
                    aux.Login = item;
                    aux.Perfil = rolTemp;
                    EEval temp = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(item.id);
                    aux.Evaluacion = temp;
                    respuesta.Add(aux);

                    //List<ELogin> Subordinados = RecuperaSubSap(item.id_sap);
                    //if (Subordinados != null)
                    //{
                    //    foreach (ELogin Sub in Subordinados)
                    //    {
                    //        Modelo.Clases.CSessionEval claseTemporal = new Modelo.Clases.CSessionEval();
                    //        EPerfil RolSub = Perfil.PerfilCls.RecuperaUnPerfil(Sub.perfil);
                    //        claseTemporal.Login = Sub;
                    //        claseTemporal.Perfil = rolTemp;
                    //        EEval EvaluacionSub = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(Sub.id);
                    //        claseTemporal.Evaluacion = EvaluacionSub;
                    //        if (EvaluacionSub != null)
                    //        {
                    //            if (EvaluacionSub.Status == 7 || EvaluacionSub.Status == 13)
                    //            {
                    //                respuesta.Add(claseTemporal);
                    //            }
                    //        }
                    //    }
                    //}
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        public static List<CSessionEval> RecuperaLiSubordinadosPeriodo(string JefeSap, int periodo)
        {
            try
            {
                List<CSessionEval> respuesta = new List<CSessionEval>();
                List<ELogin> listaUsuarios = RecuperaSubSap(JefeSap);


                foreach (ELogin item in listaUsuarios)
                {
                    CSessionEval aux = new CSessionEval();
                    EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(item.perfil);
                    //item.DivisionM = Seguridad.RecuperaDivisionPersona(item.Division);
                    item.PaisM = Pais.ClsPais.RecuperaUnPais(item.Pais);
                    aux.Login = item;
                    aux.Perfil = rolTemp;
                    EEval temp = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionPeriodoUsario(item.id, periodo);
                    aux.Evaluacion = temp;
                    respuesta.Add(aux);

                    List<ELogin> Subordinados = RecuperaSubSap(item.id_sap);
                    if (Subordinados != null)
                    {
                        foreach (ELogin Sub in Subordinados)
                        {
                            Modelo.Clases.CSessionEval claseTemporal = new Modelo.Clases.CSessionEval();
                            EPerfil RolSub = Perfil.PerfilCls.RecuperaUnPerfil(Sub.perfil);
                            claseTemporal.Login = Sub;
                            claseTemporal.Perfil = rolTemp;
                            EEval EvaluacionSub = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(Sub.id);
                            claseTemporal.Evaluacion = EvaluacionSub;
                            if (EvaluacionSub != null)
                            {
                                if (EvaluacionSub.Status == 7 || EvaluacionSub.Status == 13)
                                {
                                    respuesta.Add(claseTemporal);
                                }
                            }
                        }
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }

        public static List<CSession> RecuperaUsuarios(bool activos)
        {
            try
            {
              List<CSession> respuesta = new List<CSession>();
                List<ELogin> listaUsuarios = new List<ELogin>();
                if(activos)
                    listaUsuarios=AccesoDB.Read(new ELogin(), new List<Cliterio>()
                    {
                    new Cliterio(typeof(ELogin).GetProperty("Activo"), OperadoresRelacionales.IGUAL,true, TipoValor.Boleano)
                    });
                else
                    listaUsuarios = AccesoDB.ReadAll(new ELogin());

                foreach (ELogin item in listaUsuarios) {
                    CSession aux = new CSession();

                    EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(item.perfil);
                    EPais AuxPais = Pais.ClsPais.RecuperaUnPais(item.Pais);
                    aux.Login = item;
                    //aux.Login.DivisionM = RecuperaDivisionPersona(item.Division);
                    aux.Perfil = rolTemp;
                    aux.Pais = AuxPais;
                    respuesta.Add(aux);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        
        public static List<Modelo.Clases.CSessionEval> RecuperaLiUsuariosPais(int pais)
        {
            try
            {
                List<Modelo.Clases.CSessionEval> respuesta = new List<Modelo.Clases.CSessionEval>();
                List<ELogin> listaUsuarios = new List<ELogin>();
                if (pais != 0)
                    listaUsuarios = RecuperaUsuariosPais(pais).Where(t => t.Activo).ToList();
                else
                    listaUsuarios = RecuperaUsuariosTodos().Where(t => t.Activo).ToList();

                if (listaUsuarios != null)
                {
                    foreach (ELogin item in listaUsuarios)
                    {
                        Modelo.Clases.CSessionEval aux = new Modelo.Clases.CSessionEval();
                        EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(item.perfil);
                        //item.DivisionM = Seguridad.RecuperaDivisionPersona(item.Division);
                        aux.Login = item;
                        aux.Perfil = rolTemp;
                        EEval temp = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(item.id);
                        aux.Evaluacion = temp;
                        respuesta.Add(aux);
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        /*
        public static List<Modelo.Clases.CSessionEval> RecuperaLiUsuarios()
        {
            try
            {
                List<Modelo.Clases.CSessionEval> respuesta = new List<Modelo.Clases.CSessionEval>();
                List<ELogin> listaUsuarios = RecuperaUsuariosPais(pais);

                if (listaUsuarios != null)
                {
                    foreach (ELogin item in listaUsuarios)
                    {
                        Modelo.Clases.CSessionEval aux = new Modelo.Clases.CSessionEval();
                        EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(item.perfil);
                        aux.login = item;
                        aux.Perfil = rolTemp;
                        EEval temp = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(item.id);
                        aux.Evaluacion = temp;
                        respuesta.Add(aux);
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }*/

        public static Modelo.Clases.CSessionEval RecuperaUsuarioEval(int id_usr)
        {
            try
            {
                    ELogin usrTemp = RecuperaUnUsuario(id_usr);    
                    Modelo.Clases.CSessionEval aux = new Modelo.Clases.CSessionEval();
                    EPerfil rolTemp = Perfil.PerfilCls.RecuperaUnPerfil(usrTemp.perfil);
                    aux.Login = usrTemp;
                    aux.Perfil = rolTemp;
                    EEval temp = Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(usrTemp.id);
                if (temp != null)
                {
                    temp.periodo_etapa = Periodo.ClsPeriodo.RecuperaUnPeriodo(temp.periodo).Etapa;
                    aux.Evaluacion = temp;
                }

                return aux;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException)});
            }
        }
        
        public static string GuardaListaUsuarios(List<ELogin> ListaUsr) {
            string error = "";

            foreach (ELogin item in ListaUsr) {
                ELogin temporal = RecuperaUnUsuarioSap(item.id_sap);
                if (temporal != null) {
                    item.id = temporal.id;
                    item.Password = temporal.Password;
                }

                if (!GuardaUsuario(item)) {
                    error = error + item.id_sap + " " + item.NombreCompleto;
                }
            }

            return error;
        }
        
        public static bool GuardaUsuario(ELogin usuario)
        {
            try
            {


                if (usuario.id != 0)//modificar 
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        usuario.Operacion = TipoOperacion.Modificar;
                        OperacionUsuario(usuario, tran);
                        tran.Commit();
                    }

                    return true;
                }
                else //nueva                                                                              
                {
                    using (ITransactionCRUD tran = AccesoDB.BeginsTransaction())
                    {
                        usuario.Operacion = TipoOperacion.Nuevo;
                        //periodo.Status = 0;

                        object tmp = AccesoDB.MaxId(tran, new ELogin(), typeof(ELogin).GetProperty("id"));

                        if (DBNull.Value.Equals(tmp))
                            tmp = 0;

                        int max = (int)tmp + 1;
                        usuario.id = max;
                        usuario.Password = usuario.Password.Trim();
                        usuario.Password = General.Encrypt.Cifrado(usuario.Password);
                        OperacionUsuario(usuario, tran);
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

        private static void OperacionUsuario(ELogin Usuario, ITransactionCRUD tran)
        {
            try
            {
                switch (Usuario.Operacion)
                {
                    case TipoOperacion.Nuevo:
                        //object tmp = AccesoDB.MaxId(tran, Usuario, typeof(ELogin).GetProperty("id"));
                        //if (DBNull.Value.Equals(tmp))
                        //    tmp = 0;
                        //Usuario.UsuarioID = (int)tmp + 1;
                        AccesoDB.Save(tran, Usuario);
                        break;
                    case TipoOperacion.Lectura:
                        break;
                    case TipoOperacion.Modificar:
                        AccesoDB.Update(tran, Usuario, new List<Cliterio>()
                        {
                            new Cliterio(typeof(ELogin).GetProperty("id"), OperadoresRelacionales.IGUAL, Usuario.id, TipoValor.Numero)
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

                Usuario.Operacion = TipoOperacion.Lectura;
            }
            catch (Exception ex)
            {
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                             new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Persona: {0}", Usuario)});
            }
        }

        

    }
}
