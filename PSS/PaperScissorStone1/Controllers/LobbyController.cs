using PaperScissorStone1.Models;
using PaperScissorStoneCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaperScissorStone1.Controllers
{
    public class LobbyController : Controller
    {
        public IPlayerManager Context { get { return PlayerManager.Single; } }

        // GET: Lobby
        public ActionResult Index(int? id, string name)
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