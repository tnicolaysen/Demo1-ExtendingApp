using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GenericWebApp.Startup))]
namespace GenericWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
