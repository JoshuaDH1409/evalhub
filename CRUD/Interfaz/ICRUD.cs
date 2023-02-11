using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using General;
using System.Reflection;
using System.Data;
using CRUD.Transaction;

namespace CRUD.Interfaz
{
    public interface ICRUD
    {
        /// <summary>
        /// Inicia una transaccion
        /// </summary>
        /// <returns>Transaction CRUD</returns>
        ITransactionCRUD BeginsTransaction();
        /// <summary>
        /// Recupera los datos de la base de datos tomando en cuenta los cliterios.
        /// </summary>
        /// <param name="entidad">La entidad la cual se recuperaran los datos</param>
        /// <param name="cliterios">Los cliterios que se tomaran encuenta para la consulta</param>
        /// <returns>Lista de objectos recuperados</returns>
        List<EntidadBase> Read(EntidadBase entidad, List<Cliterio> cliterios);
        /// <summary>
        /// Recupera los datos de la base de datos tomando en cuenta los cliterios.
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">La entidad la cual se recuperaran los datos</param>
        /// <param name="cliterios">Los cliterios que se tomaran encuenta para la consulta</param>
        /// <returns>Lista de objectos recuperados</returns>
        List<EntidadBase> Read(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> cliterios);
        /// <summary>
        /// Recupera todos los datos de la base de datos.
        /// </summary>
        /// <param name="entidad">La entidad la cual se recuperaran los datos</param>
        /// <returns>Lista de objectos recuperados</returns>
        List<EntidadBase> ReadAll(EntidadBase entidad);
        /// <summary>
        /// Recupera todos los datos de la base de datos.
        /// </summary>
        /// <param name="Tran">Transacción a utilizar</param>
        /// <param name="entidad">La entidad la cual se recuperaran los datos</param>
        /// <returns>Lista de objectos recuperados</returns>
        List<EntidadBase> ReadAll(ITransactionCRUD tran, EntidadBase entidad);
        /// <summary>
        /// Guarda la informacion de la entidad.
        /// </summary>
        /// <param name="entidad">Entidad a guardar</param>
        /// <returns>True si se guardó satisfactoriamente. False si Ocurrió un error</returns>
        bool Save(EntidadBase entidad);
        /// <summary>
        /// Guarda la informacion de la entidad.
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad a guardar</param>
        /// <returns>True si se guardó satisfactoriamente. False si Ocurrió un error</returns>
        bool Save(ITransactionCRUD tran, EntidadBase entidad);
        /// <summary>
        /// Actualiza la informacion de la entidad
        /// </summary>
        /// <param name="entidad">Entidad a actualizar</param>
        /// <param name="Cliterio">Cliterios para realizar el update</param>
        /// <returns></returns>
        bool Update(EntidadBase entidad, List<Cliterio> Cliterio);
        /// <summary>
        /// Actualiza la informacion de la entidad
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad a actualizar</param>
        /// <param name="Cliterio">Cliterios para realizar el update</param>
        /// <returns></returns>
        bool Update(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio);
        /// <summary>
        /// Borra la entidad.
        /// </summary>
        /// <param name="entidad">Entidad a borrar</param>
        /// <returns></returns>
        bool delete(EntidadBase entidad);
        /// <summary>
        /// Borra la entidad.
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad a borrar</param>
        /// <returns></returns>
        bool delete(ITransactionCRUD tran, EntidadBase entidad);
        /// <summary>
        /// Borra la entidad.
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad a borrar</param>
        /// <param name="Cliterio">Cliterios para realizar el delete</param>
        /// <returns></returns>
        bool delete(ITransactionCRUD tran, EntidadBase entidad, List<Cliterio> Cliterio);
        /// <summary>
        /// Obtiene el maximo de la propiedad que se pase por parametro
        /// </summary>
        /// <param name="entidad">Entidad</param>
        /// <param name="propertycolumn">Propiedad en la cual se obtendra el maximo</param>
        /// <returns>Maximo de la propiedad</returns>
        object MaxId(EntidadBase entidad,PropertyInfo propertycolumn);
        /// <summary>
        /// Obtiene el maximo de la propiedad que se pase por parametro
        /// </summary>
        /// <param name="entidad">Entidad</param>
        /// <param name="propertycolumn">Propiedad en la cual se obtendra el maximo</param>
        /// <param name="Cliterio">Cliterios para realizar el delete</param>
        /// <returns>Maximo de la propiedad</returns>
        object MaxId(EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio);
        /// <summary>
        /// Obtiene el maximo de la propiedad que se pase por parametro
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad</param>
        /// <param name="propertycolumn">Propiedad en la cual se obtendra el maximo</param>
        /// <returns>Maximo de la propiedad</returns>
        object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn);

        /// <summary>
        /// Obtiene el maximo de la propiedad que se pase por parametro
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad</param>
        /// <param name="propertycolumn">Propiedad en la cual se obtendra el maximo</param>
        /// <returns>Maximo de la propiedad</returns>
        object MaxId(ITransactionCRUD tran, EntidadBase entidad, PropertyInfo propertycolumn, List<Cliterio> Cliterio);

        /// <summary>
        /// Borra toda la informacion de una base de datos. (Utlizar una transacción)
        /// </summary>
        /// <param name="NombreConexion">Transaccion a utilizar</param>
        /// <param name="entidad">Entidad</param>
        /// <returns></returns>
        bool deleteAllData(ITransactionCRUD tran, EntidadBase entidad);
        /// <summary>
        /// Realiza un copiado de grandes cantidades de información.
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="listentidad">Datos a pasar a la db</param>
        /// <returns>true si se ejecuto correctamente, false si Ocurrió un error</returns>
        bool BulkCopy(ITransactionCRUD tran, List<EntidadBase> listentidad);
        
        //Recuperas sentencias
        string SaveRecuperaSentencia(EntidadBase entidad);

        string UpdateRecuperaSentencia(EntidadBase entidad, List<Cliterio> Cliterio);

        /// <summary>
        /// Excuta una sentencia en una transaccion
        /// </summary>
        /// <param name="tran">Transaccion a utilizar</param>
        /// <param name="sentencia">Sentencia a ejecutar</param>
        /// <returns></returns>
        bool ExecSentencia(ITransactionCRUD tran, string sentencia);

        bool ExecSentencia(string sentencia);
    }
}
