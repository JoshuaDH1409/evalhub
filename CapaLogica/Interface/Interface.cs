using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Modelo;

namespace CapaLogica.Interface
{
    [ServiceContract]
    public interface Interface1
    {
        #region Seguridad

        [OperationContract]
        int ValidaAcceso(string usuario, string password);

        [OperationContract]
        Modelo.Clases.CSession obtenSession(string usuario, string pass);

        [OperationContract]
        List<Modelo.Clases.CSession> RecuperaUsuarios(bool activos=false);

        [OperationContract]
        List<Modelo.Clases.CSessionEval> RecuperaLiUsuariosPais(int pais);

        [OperationContract]
        ELogin RecuperaUnUsuario(int id);

        [OperationContract]
        List<ELogin> RecuperaLogInsPais(int Pais);

        [OperationContract]
        ELogin RecuperaUnUsuarioCorreo(string Correo);

        [OperationContract]
        bool GuardaUsuario(ELogin usuario);

        [OperationContract]
        ELogin RecuperaUnUsuarioSap(string idSap);

        [OperationContract]
        Modelo.Clases.CSessionEval RecuperaUsuarioEval(int id_usr);

        [OperationContract]
        List<Modelo.Clases.CSessionEval> RecuperaLiSubordinados(string JefeSap);
        [OperationContract]
        List<Modelo.Clases.CSessionEval> RecuperaLiSubordinadosPeriodo(string JefeSap, int periodo);

        [OperationContract]
        EEval RecuperaEvaluacionIdHistorial(int idEval);

        [OperationContract]
        List<EEval> RecuperaEvaluacionesPeriodo(int Periodo);


        [OperationContract]
        List<ELogin> RecuperaLogsIn();

        #endregion

        #region Perfil
        [OperationContract]
        List<EPerfil> RecuperaPerfiles();


        #endregion

        #region pais
        [OperationContract]
        List<EPais> RecuperaPaises();
        List<EDivision> RecuperaDivisiones(string term);

        [OperationContract]
        EPais RecuperaUnPais(int idPais);
        #endregion
        
        #region Excel
        [OperationContract]
        string ProcesaUsuarios(string rutaArchivo, string nombreArchivo);

        #endregion

        #region periodo
        [OperationContract]
        List<EPeriodos> RecuperaPeriodos();

        [OperationContract]
        List<EPeriodos> RecuperaTodosPeriodosPais(int Pais);

        [OperationContract]
        List<EPeriodos> RecuperaUnPeriodosActivos();

        [OperationContract]
        EPeriodos RecuperaUnPeriodo(int idPeriodo);

        [OperationContract]
        bool GuardaPeriodo(EPeriodos periodo);

        [OperationContract]
        EPeriodos RecuperaPeriodopais(int pais);

        [OperationContract]
        void EjecutarRecordatorios(int Per);
        #endregion

        #region Evaluacion

        [OperationContract]
        List<EEval> RecuperaEvaluacionesActivas();
        
        [OperationContract]
        EEval RecuperaUnaEaluacion(int id);

        [OperationContract]
        EEval RecuperaEvaluacionActivaUsario(int idUser);

        [OperationContract]
        bool GuardaEvaluacion(EEval Evaluacion);

        [OperationContract]
        List<Modelo.Clases.CHistorial> RecuperaHistorialUsr(int idUsr);

        [OperationContract]
        List<Modelo.Clases.CSessionEval> RecuperaEvaluacionesPeriodoUsuarios(int periodo);
        #endregion

        #region Objetivos
        [OperationContract]
        EObjetives RecuperaUnObjetivoid(int id);

        [OperationContract]
        List<EObjetives> RecuperaListaObjetivos(int Eval);

        [OperationContract]
        bool GuardaObjetivo(EObjetives Evaluacion);

        [OperationContract]
        bool EliminarUnObjetivo(int idObj);
        [OperationContract]
        List<EEscaleta> RecuperaEscaleta();

        [OperationContract]
        List<EEscaletaEmpresa> RecuperaEscaletaE();
        #endregion
        #region ObjetivosPais
        [OperationContract]
        EObjetivoPais RecuperaObjetivoPais(int id);

        [OperationContract]
        List<EObjetivoPais> RecuperaObjetivosPais(int pais, int periodo);

        [OperationContract]
        bool GuardaObjetivoPais(EObjetivoPais objetivo);

        [OperationContract]
        bool EliminarObjetivoPais(int idObj);
        #endregion

        #region competencias
        [OperationContract]
        List<ECompetemces> RecuperaCompetencias();

        [OperationContract]
        List<ECompetemces> RecuperaLiCompetencias(int Eval);

        [OperationContract]
        bool GuardaCompetencias(ECompetemces Competencia);

        [OperationContract]
        ECompetemces RecuperaUnaCompetencia(int id);

        [OperationContract]
        bool EliminaUnaCompetencia(int id);
        #endregion

        #region CatCompetencias
        [OperationContract]
        List<ECatComp> RecuperaCatCompetencias();

        [OperationContract]
        ECatComp RecuperaUnaCatCompetencias(int id);
        #endregion

        #region Log

        [OperationContract]
        void GuardaLogVista(List<string> listaErrores);

        #endregion

        #region Correo
        [OperationContract]
        ECorreos RecuperaUnCorreo(int id);

        [OperationContract]
        List<ECorreos> RecuperaLiCorreos();

        [OperationContract]
        bool GuardaCorreo(ECorreos correo);

        #endregion

        #region EvalView
        [OperationContract]
        List<VEvaluacion> GetEvalView();

        [OperationContract]
        List<VEvaluacion> GetEvalViewByPeriodActive();

        [OperationContract]
        List<VEvaluacion> GetEvalViewByPeriodCountry(int Periodo, int Pais);

        [OperationContract]
        List<VEvaluacion> GetEvalViewByPeriodoEvaluator(int Periodo, string IdSap);

        [OperationContract]
        List<VEvalCompetences> GetEvalCompViewByPeriodCountry(int Periodo, int Pais);

        [OperationContract]
        List<VEvalObj> GetEvalObjViewByPeriodCountryDiv(int Periodo, int Pais, string Division);
        #endregion

        #region Status
        [OperationContract]
        List<EStatus> GetStatus();

        [OperationContract]
        EStatus GetStatusById(int id);

        [OperationContract]
        bool SetStatus(EStatus modelo);
        #endregion
        #region Etapa
        [OperationContract]
        Eetapa RecuperaEtapa(int id);
        #endregion
    }

}