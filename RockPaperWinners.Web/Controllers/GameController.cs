using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using RockPaperWinners.Core;
using RockPaperWinners.Core.Entities;

namespace RockPaperWinners.Web.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        //
        // GET: /Game/

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult FindGame()
        {
            using (var context = new RockPaperWinnersContext())
            {
                // Set the current user as available
                if (!context.ActiveUsers.Where(a => a.ID == WebSecurity.CurrentUserId).Any())
                {
                    context.ActiveUsers.Add(new ActiveUser { IsInGame = false, LastActionDate = DateTime.UtcNow, UserID = WebSecurity.CurrentUserId });
                    context.SaveChanges();
                }

                // Wait for another user to become available

                // Set both users as in a game
            }

            throw new NotImplementedException();
        }

    }
}
