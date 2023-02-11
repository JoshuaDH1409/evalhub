using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PlantillaMVC.Startup))]
namespace PlantillaMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
