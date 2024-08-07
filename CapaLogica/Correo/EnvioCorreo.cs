using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo;
using System.Threading;
using System.ComponentModel;
using General;
using System.Net.Mail;
using System.Net;

namespace CapaLogica.Correo
{
    class EnvioCorreo
    {
        //mantener desactivado por requerimiento
        bool enviarCorreos = true;
        public void SendMail(string fromname, string tomail, string toname, string bodymail, string subjectmail)
        {

            if (!enviarCorreos) return;

            Task t = Task.Run(async () =>
            {
                SmtpClient client = new SmtpClient("smtp.office365.com", 0x24b)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("evaluacion.desempeno@aspenlatam.com", "Aspen2017.")
                };
                MailAddress from = new MailAddress("evaluacion.desempeno@aspenlatam.com", fromname, Encoding.UTF8);
                MailAddress to = new MailAddress(tomail);
                //to = new MailAddress("grodriguez@estrategiatec.com.mx");
                //to = new MailAddress("luis.cano@aspenlatam.com");

                MailMessage message = new MailMessage(from, to)
                {
                    Body = bodymail,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Subject = subjectmail,
                    SubjectEncoding = Encoding.UTF8
                };
                client.SendCompleted += delegate (object s, AsyncCompletedEventArgs e)
                {
                    client.Dispose();
                    message.Dispose();
                };
                string userToken = DateTime.Today.ToString();
                try
                {
                   await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                         new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Periodo: {0}", tomail)});
                }
            });

        }
        public void SendMail(string fromname, string tomail, string toname, string cc, string bodymail, string subjectmail)
        {
            if (!enviarCorreos) return;

            Task t = Task.Run(async () =>
            {
                //SmtpClient client = new SmtpClient("smtp.office365.com", 0x24b)
                //{
                //    EnableSsl = true,
                //    Credentials = new NetworkCredential("aspen.appweb@aspenlatam.com", "A5p3nW3b49")
                //};
                var fromAddress = new MailAddress("evaluacion.desempeno@aspenlatam.com", fromname);
                string fromPassword = "Aspen2017.";
                var client = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                //Creamos el objeto mensaje que será el correo
                MailMessage msg = new MailMessage();
                msg.From = fromAddress;

                msg.To.Add(tomail);
                msg.CC.Add(cc);
                msg.IsBodyHtml = true;
                msg.Body = bodymail;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Subject = subjectmail;
                msg.SubjectEncoding = Encoding.UTF8;

                client.SendCompleted += delegate (object s, AsyncCompletedEventArgs e)
                {
                    client.Dispose();
                    msg.Dispose();
                };
                string userToken = DateTime.Today.ToString();
                try
                {
                 await client.SendMailAsync(msg);
                }
                catch (Exception ex)
                {
                    throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                                      new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Periodo: {0}", tomail)});
                }
            });
        }
        public void SendMail(string fromname, List<string> tomail, string toname, string bodymail, string subjectmail)
        {
            if (!enviarCorreos) return;

            Task t = Task.Run(async () =>
            {
                //SmtpClient client = new SmtpClient("smtp.office365.com", 0x24b)
                //{
                //    EnableSsl = true,
                //    Credentials = new NetworkCredential("aspen.appweb@aspenlatam.com", "A5p3nW3b49")
                //};
                var fromAddress = new MailAddress("evaluacion.desempeno@aspenlatam.com", fromname);
                string fromPassword = "Aspen2017.";
                var client = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                if (tomail.Count > 0)
                {
                    //Creamos el objeto mensaje que será el correo
                    MailMessage msg = new MailMessage();
                    msg.From = fromAddress;

                    foreach (string mail in tomail)
                    {
                        msg.To.Add(mail);
                    }
                    msg.IsBodyHtml = true;
                    msg.Body = bodymail;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.Subject = subjectmail;
                    msg.SubjectEncoding = Encoding.UTF8;

                    client.SendCompleted += delegate (object s, AsyncCompletedEventArgs e)
                    {
                        client.Dispose();
                        msg.Dispose();
                    };
                    string userToken = DateTime.Today.ToString();
                    try
                    {
                     await client.SendMailAsync(msg);
                    }
                    catch (Exception ex)
                    {
                        throw GeneraException.AddException(ex, System.Reflection.MethodInfo.GetCurrentMethod(),
                            new string[] { string.Format("Error: {0}", ex.Message),
                                                            string.Format("InnerException: {0}", ex.InnerException),
                                                            string.Format("Periodo: {0}", tomail)});
                    }
                }
            });
        }

        public string ProcesarMsg(string Msg, Modelo.ELogin Usuario)
        {
            if (!enviarCorreos) return "";


            if (Msg.Contains("#Nombre#"))
            {
                Msg = Msg.Replace("#Nombre#",String.Concat(Usuario.NombreCompleto));
            }
            if (Msg.Contains("#Jefe#"))
            {
                ELogin jefe = Seguridad.Seguridad.RecuperaUnUsuarioSap(Usuario.EvaluadorIdSap);
                Msg = Msg.Replace("#Jefe#",String.Concat(jefe.NombreCompleto));
            }
            if (Msg.Contains("#Puesto#"))
            {
                Msg = Msg.Replace("#Puesto#", Usuario.Puesto);
            }
            if (Msg.Contains("#Area#"))
            {
                Msg = Msg.Replace("#Area#", Usuario.Division);
            }


            string Liga = General.Utilidades.ObtenerAppSettings("SelfLink");
            Liga = "[a href=\"http://" + Liga + "\"]http://" + Liga + "[/a]";
            Msg = Msg.Replace("#Liga#", Liga);

            Msg = Msg.Replace("[", "<");
            Msg = Msg.Replace("]", ">");
            return Msg;
        }

    }
}
