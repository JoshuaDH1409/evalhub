using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Spire.License;

namespace PlantillaMVC
{

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            Spire.License.LicenseProvider.SetLicenseKey("+k3AkAEA8szkluzd38nia2iuLmnoSOhKdkRFdS0asZpNtwAYGbJySc1FTdDyyC0sLaMPD30aCq9p8BuOr7koyjRFXvzHVfYHWauhBQKUKTbNbhROQ7mtoCJ90IhZ5iApxOef08UTYU5JZfeWbnd8Ie6aVvVbTVIBlNpTmkekJ1mSbC2/NIqHGenLdz/ELaxdwN/qE/Ehd7Ntad27VYbA/YGnWY4oNeQgHxyI3VIW5W7gU5ByGZ3mPM7Ir8vXqemJCdoQArRCWsdTE+7XoavBuuw2yh2hGf3b9yjXfebR6L3XTNn/5psh4EVc5dDlHn7vmK54MSVdftG9KsrM/uNZjyS1eXRwurXmT+ah7ZveNwhJ96aw8LInuh5CKXoHKG5YE45y8YkBaNLW44yzyzlZLaUP7H8VptIHMIuyy48xxFtWOTSSrMkuLIz9JQwTHbitUmTFGxS+78XubAKde9kK4RjjEVTWHFTIBlo9N1tM6/QaqrWIOEsfyCd1+bt6Q50bZqEDRDgVsuJOlzdHuAuBwk8JVRvtWp2VCUDIMbr0RCiy/Y3l1CyElnUNsjnJxywsnTDXyDEqo/DexYzoRslCQbSvNNvCIM7HlJNaGzyr9M5aMM85LEHqauye7ZgYae/zGa7EzEI289dkp8vwVUgK6TakBiYq1xyV6SOn3jG67MH2Q7+FjLmESf60fmR5Lix+znfeEyaySilHHhMn1JV5hQCiQW3UsahKZHL8wNxlt2Pxh1SyYmt9G8IWBPXaF3EkG8DvM0wfWrz1GDutLJYyj0S8Pn4+kPPEBYouieFynt/CiiL110UUkieC3zhjlsSz023w5Q8hxAlRR7RjOEFa4hwuCJbRt+jXNKu6hLO3PikCK3zXT/cqoitCGZPbZHexBjkF/GqeCHymtBKOln6v41SKBdWK+IDNMWzM1P7Q3z1tSsJUSzX/gxyT5KwpNi7dCyOvt4JPRTxIvsE4Offa0/JA3daFjUa6Sx3c9LQqCqno40kcVJt9zsiZnkXnc+/6XhgmIFH1ZgA1vN8I7b7+jtsH6bqBdqnrcmOeRipHjBA8xA/Jaap4nWibjXy8U8ZEwxc/vrBdD/tS9IJCoRnMCbKpvkX+36WoHk8ZXOVMBht5j9jTBQdr8MJ85EySEISuoQbWkfVw7SFCStqD7kQTN5WEwTL/LBpnbywrK7ZTTIm8YPv1zts8n4irrSM55K+mAxRk51vrs2RZDpH6C+9AIuvWY7m3p1SHehbvqvAztmTwsNTGZkYnpBnQbwOm+ofKEl6GiluPuiYn+I+eNe5k4hw9ZcrB4aVRJKTZIxcNG2Frg1tD9QjIB7+vJQNwdXSZVJL5I+nZu5Cb66JFo5He+eZsHEOV3LjpaylMEXWLU5ayIpCj0kh8zgcKHg+CQAErgWmwTO0c4JITS7edqeh+gw/xpWnhbeCSamYGL7v2GysyaCNcb8OFidEaDCRaHM7HP+QbHY0N8b8SQh+vxL9rBnH+1oFB0M/F6ASrkLQ1SXQ=");
            Spire.License.LicenseProvider.LoadLicense();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Verificar conexión a la base de datos
            string connectionString = ConfigurationManager.ConnectionStrings["SQLServerJoshuaLocal"].ConnectionString;
            string errorMessage;

            if (CheckDatabaseConnection(connectionString, out errorMessage))
            {
                Bitacora.NuevaEntrada("Conexión exitosa a la base de datos.", "Global.asax");
            }
            else
            {
                Bitacora.NuevaEntrada($"Error al conectar a la base de datos: {errorMessage}", "Global.asax");
                throw new Exception($"No se pudo conectar a la base de datos: {errorMessage}");
            }
        }

        private bool CheckDatabaseConnection(string connectionString, out string errorMessage)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    errorMessage = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
