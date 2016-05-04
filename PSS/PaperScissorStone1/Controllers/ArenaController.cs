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
    [ExportMetadata("ControllerName", "Arena")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ArenaController : Controller
    {
        [Import]
        public IPlayerManager Context { get; set; }
        [Import]
        public IArenaManager ArenaManager { get; set; }
        [Import]
        public IPlayerManager PlayerManager { get; set; }

        private static int GetOther(int me, IGame game)
        {
            if (me == game.LeftPlayerId)
                return game.RightPlayerId;

            return game.LeftPlayerId;
        }

        // GET: Arena
        public ActionResult Index(int gameId, int id)
        {
            var game = ArenaManager.Get(gameId);
            if (game == null)
                return RedirectToAction("Index", "Home");

            if (game.LeftPlayerId != id && game.RightPlayerId != id)
                return RedirectToAction("Index", "Home");

            var model = new ArenaViewModel() { GameId = gameId, MyId = id, TheirId = GetOther(id, game) };
            
            return View(model);
        }

        public ActionResult Stats(int gameId, int id)
        {
            var game = ArenaManager.Get(gameId);
            if (game == null)
                return RedirectToAction("Index", "Home");

            var model = new ArenaViewModel() { GameId = gameId, MyId = id, TheirId = GetOther(id, game) };

            // get statistics
            model.TurnCount = game.TurnCount;
            model.MostUsedMove = game.MostOccuringMove.ToString();

            var throws = game.Throws;
            return PartialView("Stats", model);
        }
    }
}