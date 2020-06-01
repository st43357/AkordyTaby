using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DiplomovaPrace.Startup))]
namespace DiplomovaPrace
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
