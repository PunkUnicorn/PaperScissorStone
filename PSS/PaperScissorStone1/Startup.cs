using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using PaperScissorStoneCore;
using System.ComponentModel.Composition;

[assembly: OwinStartupAttribute(typeof(PaperScissorStone1.Startup))]
namespace PaperScissorStone1
{
    public partial class Startup
    {
        [Import(typeof(IPlayerManager))]
        public IPlayerManager _dataContext { get; set; }

        [Import]
        public ILobbyConnectionMap _connectionMap { get; set; }

        [Import]
        public ILobbyChallengeMap _challengeMap { get; set; }

        public void Configuration(IAppBuilder app)
        {
            // http://www.asp.net/signalr/overview/advanced/dependency-injection
            GlobalHost.DependencyResolver.Register(
                typeof(LobbyHub),
                () => new LobbyHub(_dataContext, _connectionMap, _challengeMap));

            MefConfig.Container.ComposeParts(this);
            //Debug.Assert(MefConfig.Container != null, "expecting MefConfig.Container to be set up");
            //IDependencyResolver resolver = new MefToSignalRDependencyResolver(MefConfig.Container);
            //var config = new HubConfiguration() { Resolver = resolver };
            app.MapSignalR();// config);
        }
    }
}
