using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using PaperScissorStoneCore;
using System.ComponentModel.Composition;
using System.Diagnostics;

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

            Debug.Assert(MefConfig.Container != null, "Expecting MefConfig.Container to have been setup");
            MefConfig.Container.ComposeParts(this);
            app.MapSignalR();// config);
        }
    }
}
