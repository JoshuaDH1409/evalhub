// LIBRERÍAS MODERNAS
using Azure.Identity;
// TUS LIBRERÍAS
using CapaLogica.Log;
using DocumentFormat.OpenXml.Wordprocessing;
using General;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
// REFERENCIAS EXTRA (Asegúrate de que tu proyecto tenga referencia a Seguridad si usas esa clase)
// using Seguridad; 

namespace CapaLogica.Correo
{
    public class EnvioCorreo
    {
        // =========================================================================
        // 1. CONFIGURACIÓN Y MODO PRUEBAS
        // =========================================================================

        // ¡IMPORTANTE! Cambia esto a 'false' cuando subas a PRODUCCIÓN
        private const bool MODO_PRUEBAS = true;

        // Si MODO_PRUEBAS es true, todos los correos llegarán aquí:
        private const string CORREO_DEVELOPER = "jdiaz@estrategiatec.com.mx";

        // CREDENCIALES DE AZURE (Reemplaza con los IDs que obtuviste en el portal)
        private const string TenantId = "ed1ad1e7-8a19-4a4b-9be2-0594be3acfce";
        private const string ClientId = "fa73aa5e-32c6-417b-95d9-9fab46ca54fa";
        private const string ClientSecret = "T4w8Q~nR_GevYYmZeUn_iMwJadBMroLYAOQTMb8h";

        // El buzón compartido desde el que salen los correos
        private const string RemitenteOficial = "evaluacion.desempeno@aspenlatam.com";

        // =========================================================================
        // 2. MÉTODOS PÚBLICOS (IGUALES A TU CÓDIGO ORIGINAL)
        // =========================================================================

        // MÉTODO 1: Envío simple a un destinatario
        //public void SendMail(string fromname, string tomail, string toname, string bodymail, string subjectmail)
        //{
        //    // Mantenemos Task.Run para no bloquear el hilo principal (compatibilidad legacy)
        //    Task.Run(async () =>
        //    {
        //        try
        //        {
        //            // Convertimos el string único a lista y llamamos al motor
        //            var toList = new List<string> { tomail };
        //            await SendMailGraphAsync(toList, null, subjectmail, bodymail);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError(ex, tomail);
        //        }
        //    });
        //}


        public void SendMail(string fromname, string tomail, string toname, string bodymail, string subjectmail)
        {
            // Quitamos Task.Run TEMPORALMENTE para intentar atrapar el error, 
            // o usamos un Log en archivo de texto.
            Task.Run(async () =>
            {
                try
                {
                    var toList = new List<string> { tomail };
                    await SendMailGraphAsync(toList, null, subjectmail, bodymail);

                    // Si llega aquí, es que SÍ salió. 
                    System.IO.File.AppendAllText(@"C:\Temp\LogCorreo.txt",
                        $"[{DateTime.Now}] ÉXITO: Correo enviado a {tomail} (o al developer si modo pruebas activado)\n");
                }
                catch (Exception ex)
                {
                    // ESTO ES LO QUE NECESITAMOS VER
                    string errorMsg = $"[{DateTime.Now}] ERROR CRÍTICO enviando a {tomail}:\n{ex.Message}\n{ex.InnerException?.Message}\n----------------\n";

                    // Intenta escribir en C:\Temp (asegurate que la carpeta exista)
                    try { System.IO.File.AppendAllText(@"C:\Temp\LogCorreo.txt", errorMsg); } catch { }

                    // También mantenemos tu log original
                    LogError(ex, tomail);
                }
            });
        }

        // MÉTODO 2: Envío con Copia (CC)
        public void SendMail(string fromname, string tomail, string toname, string cc, string bodymail, string subjectmail)
        {
            Task.Run(async () =>
            {
                try
                {
                    var toList = new List<string> { tomail };
                    // Validamos si hay CC antes de crear la lista
                    var ccList = !string.IsNullOrEmpty(cc) ? new List<string> { cc } : null;

                    await SendMailGraphAsync(toList, ccList, subjectmail, bodymail);
                }
                catch (Exception ex)
                {
                    LogError(ex, tomail);
                }
            });
        }

        // MÉTODO 3: Envío a Lista de Correos
        public void SendMail(string fromname, List<string> tomail, string toname, string bodymail, string subjectmail)
        {
            if (tomail != null && tomail.Count > 0)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await SendMailGraphAsync(tomail, null, subjectmail, bodymail);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, string.Join(",", tomail));
                    }
                });
            }
        }

        // =========================================================================
        // 3. MOTOR CENTRAL (OAUTH2 + LOGICA DE PRUEBAS)
        // =========================================================================
        private async Task SendMailGraphAsync(List<string> toList, List<string> ccList, string subject, string body)
        {

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            // A. Autenticación con Azure AD
            var options = new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud };
            var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret, options);
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            // B. Lógica de "Modo Pruebas" (Interceptar destinatarios)
            List<Recipient> destinatariosFinales = new List<Recipient>();
            List<Recipient> copiasFinales = new List<Recipient>();

            if (MODO_PRUEBAS)
            {
                // Si estamos probando, ignoramos los destinatarios reales y lo mandamos al Developer
                destinatariosFinales.Add(new Recipient { EmailAddress = new EmailAddress { Address = CORREO_DEVELOPER } });

                // Agregamos una nota visual al cuerpo del correo para saber a quién iba
                string notaPrueba = $"<br><hr><strong style='color:red;'>[MODO PRUEBA ACTIVO]</strong><br>" +
                                    $"Este correo se intentó enviar originalmente a: <b>{string.Join(", ", toList)}</b>";
                if (ccList != null) notaPrueba += $"<br>Con copia a: <b>{string.Join(", ", ccList)}</b>";

                body = body + notaPrueba;
                subject = "[TEST] " + subject;
            }
            else
            {
                // Modo Producción: Usamos los destinatarios reales
                if (toList != null)
                    destinatariosFinales = toList.Select(e => new Recipient { EmailAddress = new EmailAddress { Address = e } }).ToList();

                if (ccList != null)
                    copiasFinales = ccList.Select(e => new Recipient { EmailAddress = new EmailAddress { Address = e } }).ToList();
            }

            // C. Construcción del Objeto Mensaje (Graph v5)
            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Microsoft.Graph.Models.Message
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        ContentType = Microsoft.Graph.Models.BodyType.Html,
                        Content = body
                    },
                    ToRecipients = destinatariosFinales,
                    CcRecipients = copiasFinales
                },
                SaveToSentItems = false // No llenar el buzón compartido de elementos enviados
            };

            // D. Envío
            await graphClient.Users[RemitenteOficial]
                .SendMail
                .PostAsync(requestBody);
        }

        // =========================================================================
        // 4. UTILIDADES Y PROCESAMIENTO
        // =========================================================================

        private void LogError(Exception ex, string info)
        {
            try
            {
                // Usamos tu clase GeneraException original
                throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                    new string[] { $"Error: {ex.Message}", $"Inner: {ex.InnerException}", $"Destino: {info}" });
            }
            catch
            {
                // Fallback a consola si falla el log de base de datos
                System.Diagnostics.Debug.WriteLine($"Error crítico enviando mail a {info}: {ex.Message}");
            }
        }

        public string ProcesarMsg(string Msg, Modelo.ELogin Usuario)
        {
            if (Msg.Contains("#Nombre#"))
            {
                Msg = Msg.Replace("#Nombre#", String.Concat(Usuario.NombreCompleto));
            }

            // Mantenemos tu lógica original, pero protegida con try-catch por si falla la referencia 'Seguridad'
            if (Msg.Contains("#Jefe#"))
            {
                try
                {
                    // Asegúrate de que tengas el 'using Seguridad;' arriba o el namespace correcto
                    // Si te marca error aquí, verifica que el proyecto tenga referencia a la DLL de Seguridad
                    Modelo.ELogin jefe = Seguridad.Seguridad.RecuperaUnUsuarioSap(Usuario.EvaluadorIdSap);
                    Msg = Msg.Replace("#Jefe#", String.Concat(jefe.NombreCompleto));

                    // NOTA: Si sigue fallando la referencia, comenta las 2 lineas de arriba y descomenta esta:
                   // Msg = Msg.Replace("#Jefe#", "Jefe (Ref Pendiente)");
                }
                catch { }
            }

            if (Msg.Contains("#Puesto#")) Msg = Msg.Replace("#Puesto#", Usuario.Puesto);
            if (Msg.Contains("#Area#")) Msg = Msg.Replace("#Area#", Usuario.Division);

            try
            {
                string Liga = General.Utilidades.ObtenerAppSettings("SelfLink");
                Liga = "[a href=\"http://" + Liga + "\"]http://" + Liga + "[/a]";
                Msg = Msg.Replace("#Liga#", Liga);
            }
            catch { }

            Msg = Msg.Replace("[", "<");
            Msg = Msg.Replace("]", ">");
            return Msg;
        }
    }
}