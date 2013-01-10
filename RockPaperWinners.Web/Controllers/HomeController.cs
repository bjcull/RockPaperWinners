using RockPaperWinners.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RockPaperWinners.Core.Entities;
using WebMatrix.WebData;
using GravatarHelper;

namespace RockPaperWinners.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var model = new HomeViewModel();

            using (var context = new RockPaperWinnersContext())
            {
                //var user = context.UserProfiles.Find(WebSecurity.CurrentUserId);

                //model.CurrentFunds = user.Money;
                //model.Email = user.Email;
                //model.Name = user.FullName;
                //model.ImageSource = GravatarHelper.GravatarHelper.CreateGravatarUrl(user.Email, 128, "Default",null, null, null);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
