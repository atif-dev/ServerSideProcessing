using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ServerSideProcessing.Startup))]
namespace ServerSideProcessing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
