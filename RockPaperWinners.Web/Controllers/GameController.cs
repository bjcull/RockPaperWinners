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

                // Record the current datetimeutc, set variable to be certain time in future (+2 mins)

                // BEGIN TRAN

                // While not timed out, check the game result entities to find any active game results with my id

                // If found, return the active game

                // Else, check the active users table for an available to pair

                // If found, create the game result, add players and return the game

                // END TRAN

                // Else, continue

                // If after 2 minutes, return message

                // Set both users as in a game
            }

            throw new NotImplementedException();
        }

    }
}
