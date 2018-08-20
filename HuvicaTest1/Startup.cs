using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CucuruchoWeb.Startup))]
namespace CucuruchoWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
