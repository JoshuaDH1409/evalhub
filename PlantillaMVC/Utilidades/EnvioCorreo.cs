using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CapaLogica.Log;
using General;
using Modelo;

namespace PlantillaMVC
{
    public class EnvioCorreo
    {

        public void SendMail(string fromname, string tomail, string toname, string bodymail, string subjectmail)
        {
            bool enviarCorreos = false;

            if (enviarCorreos)
            {
                ThreadPool.QueueUserWorkItem(delegate (object t)
                {
                    SmtpClient client = new SmtpClient("smtp.office365.com", 0x24b)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential("evaluacion.desempeno@aspenlatam.com", "Aspen2017.")
                    };
                    MailAddress from = new MailAddress("evaluacion.desempeno@aspenlatam.com", fromname, Encoding.UTF8);
                    MailAddress to = new MailAddress(tomail);
                    //MailAddress to = new MailAddress("fvargas@estrategiatec.com.mx");
                    //MailAddress to = new MailAddress("giselle.dominguez@aspenlatam.com");
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
                        Thread.Sleep(5000);
                        client.SendAsync(message, userToken);
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

                
        }

     
        public string ProcesarMsg(string Msg, ELogin Usuario)
        {
           

            if (Msg.Contains("#Nombre#"))
            {
                Msg = Msg.Replace("#Nombre#",String.Concat(String.Concat(Usuario.Nombre+" "+Usuario.ApellidoPat+" "+Usuario.ApellidoMat)));
            }
            if (Msg.Contains("#Jefe#"))
            {
                ELogin jefe = Utilidades.negocio.RecuperaUnUsuarioSap(Usuario.EvaluadorIdSap);
                Msg = Msg.Replace("#Jefe#", String.Concat(jefe.NombreCompleto));
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

        #region emailNoUtilizados
        public async Task SendEmailAsync(Models.Email email)
        {
            string mailAddressTo = "evaluacion.desempeno@aspenlatam.com";
            string mailAddressPass = "Aspen2017.";

            var fromAddress = new MailAddress(mailAddressTo, "Aspen - Evaluación de Desempeño");
            string fromPassword = mailAddressPass;
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
            //msg.To.Add(email.ToEmail);
            if (email.ToEmail != null)
            {
                foreach (string tomail in email.ToEmail)
                {
                    msg.To.Add(tomail);
                }
            }
            if (email.CC != null)
            {
                foreach (string cc in email.CC)
                {
                    msg.CC.Add(cc);
                }
            }
            if (email.BCC != null)
            {
                foreach (string bcc in email.BCC)
                {
                    msg.Bcc.Add(bcc);
                }
            }
            msg.IsBodyHtml = true;
            msg.Body = email.Body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.Subject = email.Subject;
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        public async Task SendEmailAsync(string fromname, string tomail, string toname, string cc, string bodymail, string subjectmail)
        {
            string mailAddressTo = "evaluacion.desempeno@aspenlatam.com";
            string mailAddressPass = "Aspen2017.";

            var fromAddress = new MailAddress(mailAddressTo, fromname);
            string fromPassword = mailAddressPass;
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
            //msg.To.Add("giselle.dominguez@aspenlatam.com");
            //msg.To.Add("fvargas@estrategiatec.com.mx");
            if (cc != "")
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        /*public void SendEmail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);

            message.Subject = emailSubject;
            message.Body = emailMessage;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = "smtp.office365.com";
                smtpClient.Port = 587;
                NetworkCredential cred = new System.Net.NetworkCredential("evaluacion.desempeno@aspenlatam.com", "Aspen2017.");
                smtpClient.Credentials = cred;
                smtpClient.EnableSsl = true;

                smtpClient.SendMailAsync(message);
            }
        }*/

        public void SendMail(string fromname, string tomail, string toname, string cc, string bodymail, string subjectmail)
        {
            ThreadPool.QueueUserWorkItem(delegate (object t)
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
                //msg.To.Add("fvargas@estrategiatec.com.mx");
                //msg.To.Add("giselle.dominguez@aspenlatam.com");
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
                    client.SendAsync(msg, userToken);
                    Thread.Sleep(5000);
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
            ThreadPool.QueueUserWorkItem(delegate (object t)
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
                        client.SendAsync(msg, userToken);
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

        #endregion

    }
}