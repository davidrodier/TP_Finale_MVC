using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TP_Finale_David_Rodier.Startup))]
namespace TP_Finale_David_Rodier
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
