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
    [ExportMetadata("ControllerName", "Home")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller
    {
        private IPlayerManager Context { get; set; }

        [ImportingConstructor]
        public HomeController(IPlayerManager context)
        {
            Context = context;
        }

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
                model.Errors = errorList;
                model.Name = string.Empty;
                return View("Index", model);
            }

            int? id = null;
            if (model.IsLogin)
            {
                id = Context.LogOn(model.Name, model.Password);
            }
            else
            {
                if (model.Password != model.ConfirmPassword)
                {
                    errorList.Add("Passwords do not match");
                    model.Errors = errorList;
                    return View("Index", model);
                }

                id = Context.Register(model.Name, model.Password);
            }

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