using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeliveryMan.Startup))]
namespace DeliveryMan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
