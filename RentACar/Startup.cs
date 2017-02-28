using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RentACar.Startup))]
namespace RentACar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
