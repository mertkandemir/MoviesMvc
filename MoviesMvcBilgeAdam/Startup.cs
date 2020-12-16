using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoviesMvcBilgeAdam.Startup))]
namespace MoviesMvcBilgeAdam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
