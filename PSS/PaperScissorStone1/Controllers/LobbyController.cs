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

            return View(new LobbyViewModel { Id = id.Value, Name = name, Players = Context.LoggedOn });
        }
    }
}