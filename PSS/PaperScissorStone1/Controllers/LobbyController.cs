using PaperScissorStone1.Models;
using PaperScissorStoneCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaperScissorStone1.Controllers
{
    [Export]
    [ExportMetadata("ControllerName", "Lobby")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LobbyController : Controller
    {
        [Import]
        public IArenaManager Arena { get; set; }

        private IPlayerManager Context { get; set; }

        [ImportingConstructor]
        public LobbyController(IPlayerManager context)
        {
            Context = context;
        }

        // GET: Lobby
        public ActionResult Index(int? id = null, string name = null)
        {
            if (!id.HasValue || string.IsNullOrWhiteSpace(name))
                return RedirectToAction("Index", "Home");

            bool loggedOn = Context.LoggedOn.Any(a => a.Id == id);
            if (!loggedOn)
                return RedirectToAction("Index", "Home", name);

            var model = new LobbyViewModel
            {
                Id = id.Value,
                Name = name
            };

            var lobbyPlayersIds = Context.LoggedOn.
                    Select(s => s.Id).
                    Distinct().
                    Except(Arena.Players);

            model.Players = Context.LoggedOn.
                    Where(w => lobbyPlayersIds.Contains(w.Id)).
                    ToList();

            return View(model);
        }
    }
}