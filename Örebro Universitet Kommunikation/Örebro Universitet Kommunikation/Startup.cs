using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Örebro_Universitet_Kommunikation.Startup))]
namespace Örebro_Universitet_Kommunikation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
