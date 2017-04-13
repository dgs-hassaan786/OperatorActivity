using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Operational.Portal.Startup))]
namespace Operational.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
