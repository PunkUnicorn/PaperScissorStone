using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PaperScissorStone1.Startup))]
namespace PaperScissorStone1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
