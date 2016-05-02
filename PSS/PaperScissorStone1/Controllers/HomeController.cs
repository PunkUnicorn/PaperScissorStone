using PaperScissorStone1.Models;
using PaperScissorStone1.Models.Home;
using PaperScissorStoneCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaperScissorStone1.Controllers
{
    public class HomeController : Controller
    {
        public IPlayerManager Context { get { return PlayerManager.Single; } }

        public ActionResult Index(HomeViewModel model)
        {
            if (model == null)
                model = new HomeViewModel();

            return View(model);
        }

        public ActionResult LoginRegister(HomeViewModel model)
        {
            var errorList = new List<string>();
           
            if (!ModelState.IsValid)
                return View("Index", model);

            // Check for duplicate names if regstering
            if (!model.IsLogin &&
                Context.IsDuplicateName(model.Name))
            {
                errorList.Add(string.Format("The name {0} is alread used", model.Name));
                model.Name = string.Empty;
                return View("Index", model);
            }

            int? id = null;
            if (model.IsLogin)
                id = Context.LogOn(model.Name, model.Password);
            else
                id = Context.Register(model.Name, model.Password);

            if (id.HasValue)
                return RedirectToAction("Index", "Lobby", new LobbyViewModel() { Id = id.Value, Name = model.Name });

            return View("Index", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}