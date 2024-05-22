using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modelo;
using CapaLogica.Seguridad;
using General;
using CapaLogica.Log;
using Modelo.Clases;

namespace CapaLogica
{

    public class CapaLogica : Interface.Interface1
    {
        #region Seguridad
        public Modelo.Clases.CSession obtenSession(string usuario, string password)
        {
            try
            {
                return Seguridad.Seguridad.obtenSession(usuario, password);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public int ValidaAcceso(string usuario, string pass)
        {
            try
            {

                return Seguridad.Seguridad.ValidaAcceso(usuario, pass);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return 0;
            }
        }

        public List<CSession> RecuperaUsuarios(bool activos)
        {
            try
            {

                return Seguridad.Seguridad.RecuperaUsuarios(activos);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<Modelo.Clases.CSessionEval> RecuperaLiUsuariosPais(int pais)
        {
            try
            {

                return Seguridad.Seguridad.RecuperaLiUsuariosPais(pais);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public ELogin RecuperaUnUsuario(int id)
        {
            try
            {

                return Seguridad.Seguridad.RecuperaUnUsuario(id);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<ELogin> RecuperaLogInsPais(int Pais)
        {
            try
            {
                return Seguridad.Seguridad.RecuperaLogInsPais(Pais);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public ELogin RecuperaUnUsuarioCorreo(string Correo)
        {
            try
            {

                return Seguridad.Seguridad.RecuperaUnUsuarioCorreo(Correo);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public ELogin RecuperaUnUsuario(string Correo)
        {
            try
            {

                return Seguridad.Seguridad.RecuperaUnUsuarioCorreo(Correo);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool GuardaUsuario(ELogin usuario)
        {
            try
            {

                return Seguridad.Seguridad.GuardaUsuario(usuario);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public ELogin RecuperaUnUsuarioSap(string idSap)
        {
            try
            {
                return Seguridad.Seguridad.RecuperaUnUsuarioSap(idSap);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public Modelo.Clases.CSessionEval RecuperaUsuarioEval(int id_usr)
        {
            try
            {
                return Seguridad.Seguridad.RecuperaUsuarioEval(id_usr);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<Modelo.Clases.CSessionEval> RecuperaLiSubordinados(string JefeSap)
        {
            try
            {
                return Seguridad.Seguridad.RecuperaLiSubordinados(JefeSap);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<Modelo.Clases.CSessionEval> RecuperaLiSubordinadosPeriodo(string JefeSap, int periodo)
        {
            try
            {
                return Seguridad.Seguridad.RecuperaLiSubordinadosPeriodo(JefeSap, periodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public List<ELogin> RecuperaLogsIn()
        {
            try
            {
                return Seguridad.Seguridad.RecuperaLogsIn();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        #endregion

        #region rol
        public List<EPerfil> RecuperaPerfiles()
        {
            try
            {
                return Perfil.PerfilCls.RecuperaPerfil();
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EPerfil RecuperaUnPerfil(int idPerfil)
        {
            try
            {
                return Perfil.PerfilCls.RecuperaUnPerfil(idPerfil);
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        #endregion

        #region pais
        public List<EPais> RecuperaPaises()
        {
            try
            {
                return Pais.ClsPais.RecuperaPaises();
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<EDivision> RecuperaDivisiones(string term)
        {
            try
            {
                return Pais.ClsPais.RecuperaDivisiones(term);
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EPais RecuperaUnPais(int idPais)
        {
            try
            {
                return Pais.ClsPais.RecuperaUnPais(idPais);
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public Eetapa RecuperaEtapa(int id)
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaEtapa(id);
            }
            catch (ListErrores ex)
            {

                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        #endregion

        #region Excel
        public string ProcesaUsuarios(string rutaArchivo, string nombreArchivo)
        {
            try
            {
                Excel.ClsExcel constructor = new Excel.ClsExcel();
                return constructor.ProcesaUsuarios( rutaArchivo, nombreArchivo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        #endregion
        
        #region Periodo
        public List<EPeriodos> RecuperaPeriodos()
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaPeriodos();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<EPeriodos> RecuperaTodosPeriodosPais(int Pais)
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaTodosPeriodosPais(Pais);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public List<EPeriodos> RecuperaUnPeriodosActivos()
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaUnPeriodosActivos();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EPeriodos RecuperaUnPeriodo(int idPeriodo)
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaUnPeriodo(idPeriodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        
        public bool GuardaPeriodo(EPeriodos periodo)
        {
            try
            {
                return Periodo.ClsPeriodo.GuardaPeriodo(periodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public EPeriodos RecuperaPeriodopais(int pais)
        {
            try
            {
                return Periodo.ClsPeriodo.RecuperaPeriodopais(pais);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EEval RecuperaEvaluacionIdHistorial(int idEval)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaEvaluacionIdHistorial(idEval);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        
        public List<EEval> RecuperaEvaluacionesPeriodo(int Periodo)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodo(Periodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public void EjecutarRecordatorios(int Per)
        {
            try
            {
                Periodo.ClsPeriodo Constructor = new Periodo.ClsPeriodo();
                Constructor.EjecutarRecordatorios(Per);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
            }
        }

        #endregion

        #region Evaluacion
        public List<EEval> RecuperaEvaluacionesActivas()
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesActivas();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EEval RecuperaUnaEaluacion(int id)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaUnaEaluacion(id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }




        public EEval RecuperaEvaluacionActivaUsario(int idUser)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaEvaluacionActivaUsario(idUser);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public bool GuardaEvaluacion(EEval Evaluacion)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GuardaEvaluacion(Evaluacion);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public List<Modelo.Clases.CHistorial> RecuperaHistorialUsr(int idUsr)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaHistorialUsr(idUsr);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<Modelo.Clases.CSessionEval> RecuperaEvaluacionesPeriodoUsuarios(int periodo)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.RecuperaEvaluacionesPeriodoUsuarios(periodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        #endregion

        #region Objetivos

        public EObjetives RecuperaUnObjetivoid(int id)
        {
            try
            {
                return Objetivos.ClsObjetivos.RecuperaUnObjetivoId(id);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<EObjetives> RecuperaListaObjetivos(int Eval)
        {
            try
            {
                return Objetivos.ClsObjetivos.RecuperaListaObjetivos(Eval);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool GuardaObjetivo(EObjetives Evaluacion)
        {
            try
            {
                return Objetivos.ClsObjetivos.GuardaObjetivo(Evaluacion);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }


        public bool EliminarUnObjetivo(int idObj)
        {
            try
            {
                return Objetivos.ClsObjetivos.EliminaUnObjetivo(idObj);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }
        public List<EEscaleta> RecuperaEscaleta()
        {
            try
            {
                return Objetivos.ClsObjetivos.RecuperaEscaleta();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<EEscaletaEmpresa> RecuperaEscaletaE()
        {
            try
            {
                return Objetivos.ClsObjetivos.RecuperaEscaletaE();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        #endregion
        #region ObjetivosPais

        public EObjetivoPais RecuperaObjetivoPais(int id)
        {
            try
            {
                return ObjetivosPais.ClsObjetivosPais.RecuperaObjetivo(id);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<EObjetivoPais> RecuperaObjetivosPais(int pais, int periodo)
        {
            try
            {
                return ObjetivosPais.ClsObjetivosPais.RecuperaObjetivos(pais, periodo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool GuardaObjetivoPais(EObjetivoPais Evaluacion)
        {
            try
            {
                return ObjetivosPais.ClsObjetivosPais.GuardaObjetivo(Evaluacion);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }


        public bool EliminarObjetivoPais(int idObj)
        {
            try
            {
                return ObjetivosPais.ClsObjetivosPais.EliminaObjetivo(idObj);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }


        #endregion

        #region Competencias


        public List<ECompetemces> RecuperaCompetencias()
        {
            try
            {
                return Competencias.ClsCompetencias.RecuperaCompetencias();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public  List<ECompetemces> RecuperaLiCompetencias(int Eval)
        {
            try
            {
                return Competencias.ClsCompetencias.RecuperaLiCompetencias(Eval);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public  bool GuardaCompetencias(ECompetemces Competencia)
        {
            try
            {
                return Competencias.ClsCompetencias.GuardaCompetencias(Competencia);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public  ECompetemces RecuperaUnaCompetencia(int id)
        {
            try
            {
                return Competencias.ClsCompetencias.RecuperaUnaCompetencia(id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public bool EliminaUnaCompetencia(int id)
        {
            try
            {
                return Competencias.ClsCompetencias.EliminaUnaCompetencia(id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }


        #endregion

        #region catCompetencias

        public  List<ECatComp> RecuperaCatCompetencias()
        {
            try
            {
                return CatCompe.ClsCatComp.RecuperaCatCompetencias();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<ECatSubComp> RecuperaCatSubCompetencias()
        {
            try
            {
                return CatCompe.ClsCatComp.RecuperaCatSubCompetencias();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }



        
        public ECatComp RecuperaUnaCatCompetencias(int id)
        {
            try
            {
                return CatCompe.ClsCatComp.RecuperaUnaCompetencia(id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public ECatSubComp RecuperaCatSubCompetencia(int Id)
        {
            try
            {
                return CatCompe.ClsCatComp.RecuperaUnaSubCompetencia(Id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        #endregion

        #region Objetivos Personales PTP
        public List<EPersonalTP> RecuperaListaObjetivosPTP(int Eval)
        {
            try
            {
                return ObjetivosPTP.ClsEObjetivesPersonalTP.RecuperaListaObjetivos(Eval);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EPersonalTP RecuperaUnObjetivoPTPid(int id)
        {
            try
            {
                
                return ObjetivosPTP.ClsEObjetivesPersonalTP.RecuperaUnObjetivoPTPid(id);

            }catch(ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool GuardaObjetivosPTP(EPersonalTP objetivo)
        {
            try
            {
                return ObjetivosPTP.ClsEObjetivesPersonalTP.GuardaObjetivo(objetivo);
            }catch(ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public bool EliminaUnObjetivoPTP(int id)
        {
            try
            {
                return ObjetivosPTP.ClsEObjetivesPersonalTP.EliminaUnObjetivo(id);

            }
            catch(ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        #endregion

        #region OBJETIVOS PDP

        public List<EPersonalDP> RecuperaListaObjetivosPDP(int Eval)
        {
            try
            {
                return ObjetivosPDP.ClsEObjetivesPersonalDP.RecuperaListaObjetivos(Eval);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public EPersonalDP RecuperaUnObjetivoPDPid(int id)
        {
            try
            {

                return ObjetivosPDP.ClsEObjetivesPersonalDP.RecuperaUnObjetivoPTPid(id);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool GuardaObjetivosPDP(EPersonalDP objetivo)
        {
            try
            {
                return ObjetivosPDP.ClsEObjetivesPersonalDP.GuardaObjetivo(objetivo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        public bool EliminaUnObjetivoPDP(int id)
        {
            try
            {
                return ObjetivosPDP.ClsEObjetivesPersonalDP.EliminaUnObjetivo(id);

            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }


        #endregion

        #region Correos

        public ECorreos RecuperaUnCorreo(int id)
        {
            try
            {
                return Correo.ClsCorreos.RecuperaUnCorreo(id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }


        public List<ECorreos> RecuperaLiCorreos()
        {
            try
            {
                return Correo.ClsCorreos.RecuperaCorreos();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }



        public bool GuardaCorreo(ECorreos correo)
        {
            try
            {
                return Correo.ClsCorreos.GuardaCorreo(correo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }

        #endregion

        #region log 
        public void GuardaLogVista(List<string> listaErrores)
        {
            try
            {
                Log.LogErrores.GuardaLogVsta(listaErrores);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));              
            }
        }
        #endregion

        #region EvalView
        public List<VEvaluacion> GetEvalView()
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalView();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<VEvaluacion> GetEvalViewByPeriodActive()
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalViewByPeriodActive();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<VEvaluacion> GetEvalViewByPeriodCountry(int Periodo, int Pais)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalViewByPeriodCountry(Periodo, Pais);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<VEvaluacion> GetEvalViewByPeriodoEvaluator(int Periodo, string IdSap)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalViewByPeriodoEvaluator(Periodo, IdSap);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public List<VEvalCompetences> GetEvalCompViewByPeriodCountry(int Periodo, int Pais)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalCompViewByPeriodCountry(Periodo, Pais);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public List<VEvalObj> GetEvalObjViewByPeriodCountryDiv(int Periodo, int Pais, string Division)
        {
            try
            {
                return Evaluaciones.ClsEvaluacion.GetEvalObjViewByPeriodCountryDiv(Periodo, Pais, Division);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        #endregion
        #region Status
        public List<EStatus> GetStatus()
        {
            try
            {
                return Status.ClsStatus.GetStatus();
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }
        public EStatus GetStatusById(int Id)
        {
            try
            {
                return Status.ClsStatus.GetStatusById(Id);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return null;
            }
        }

        public bool SetStatus(EStatus modelo)
        {
            try
            {
                return Status.ClsStatus.SetStatus(modelo);
            }
            catch (ListErrores ex)
            {
                LogErrores.GuardaLogCapaLogica(GeneraException.RecuperaErrores(ex));
                return false;
            }
        }
        #endregion
    }
}
