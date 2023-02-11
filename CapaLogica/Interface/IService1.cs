using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Modelo;

namespace CapaLogica.Interface
{
    public interface IService1
    {
        #region Seguridad
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        ELogin GetDataUsingDataContract(ELogin Login);

        [OperationContract]
        bool VerificaUsuario(ELogin login);
        #endregion

        //[OperationContract]
        //CSession UsuarioValido(ELogin login);

    }
}
