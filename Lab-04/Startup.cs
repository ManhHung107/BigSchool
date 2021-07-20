using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lab_04.Startup))]
namespace Lab_04
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
