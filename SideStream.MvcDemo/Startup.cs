using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SideStream.MvcDemo.Startup))]
namespace SideStream.MvcDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
