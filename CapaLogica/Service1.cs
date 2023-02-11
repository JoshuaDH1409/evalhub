using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaLogica.Interface;
using Modelo;

namespace CapaLogica
{
    class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public ELogin GetDataUsingDataContract(ELogin Login)
        {
            if (Login == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (Login.id == 0)
            {
                Login.Email += "Suffix";
            }
            return Login;
        }

        public bool VerificaUsuario(ELogin login)
        {
            throw new Exception("Error si pasa el mensaje");
        }

        //public CSession UsuarioValido(ELogin login)
        //{

        //    throw new Exception("Error si pasa el mensaje");
        //}
    }
}
