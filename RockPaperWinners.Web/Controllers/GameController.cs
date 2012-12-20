using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using RockPaperWinners.Core;
using RockPaperWinners.Core.Entities;
using System.Transactions;
using RockPaperWinners.Web.Models;
using System.Threading;

namespace RockPaperWinners.Web.Controllers
{
    public class GameController : BaseController
    {
        //
        // GET: /Game/

        [HttpGet]
        public ActionResult Index()
        {
            using (var db = new RockPaperWinnersContext())
            {
                db.GameResults.Add(new GameResult { GameDateTime = DateTime.UtcNow, IsActive = true });
                db.SaveChanges();
            }

            return View();
        }

        [HttpGet]
        public JsonResult FindGame()
        {
            // Model to return if game found
            var model = new GameViewModel();

            // Transaction scope
            //var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
            //    new TransactionOptions()
            //    {
            //        IsolationLevel = IsolationLevel.ReadCommitted
            //    });


            // Begin the transaction
            //using (scope)
            //{

                // Initialise the context
                using (var context = new RockPaperWinnersContext())
                {
                    // Set the current user as available
                    if (!context.ActiveUsers.Where(a => a.ID == WebSecurity.CurrentUserId).Any())
                    {
                        context.ActiveUsers.Add(new ActiveUser { IsInGame = false, LastActionDate = DateTime.UtcNow, UserID = WebSecurity.CurrentUserId });
                        context.SaveChanges();
                    }

                    // Record the current datetimeutc, set variable to be a time when we will give up trying to find someone
                    DateTime waitUntilTime = DateTime.UtcNow.AddMinutes(2);

                    // BEGIN TRAN

                    // While not timed out, check the game result entities to find any active game results with my id
                    while (DateTime.UtcNow <= waitUntilTime)
                    {
                        // Get the current user
                        var me = context.UserProfiles.Find(WebSecurity.CurrentUserId);

                        // Look to see if there are any active games for the current user
                        var activeGame = (from g in context.GameResults
                                          join gp in context.GameResultPlayers
                                          on g.ID equals gp.GameResultID
                                          where g.IsActive && gp.UserID == me.ID
                                          select g).FirstOrDefault();

                        if (activeGame != null)
                        {

                            var game = context.GameResults.Include("GameResultPlayers").Where(g => g.ID == activeGame.ID).FirstOrDefault();

                            // Find the opponent
                            var opponent = game.GameResultPlayers.Where(gp => gp.UserID != me.ID).FirstOrDefault();
                            var myGame = game.GameResultPlayers.Where(gp => gp.UserID == me.ID).FirstOrDefault();

                            // Build the model
                            // TODO: Generate random bet amounts
                            model = new GameViewModel
                            {
                                GameResultID = activeGame.ID,
                                MyUserID = me.ID,
                                OpponentID = opponent.UserID,
                                MyBetAmount = myGame.BetAmount,
                                OpponentBetAmount = opponent.BetAmount,
                                MyName = me.FullName,
                                OpponentName = context.UserProfiles.Find(opponent.UserID).FullName,
                                TotalMoney = me.Money
                            };

                            // Set the current user to be in a game
                            var activeUser = context.ActiveUsers.Where(a => a.UserID == me.ID).FirstOrDefault();
                            activeUser.IsInGame = true;
                            activeUser.LastActionDate = DateTime.UtcNow;

                            context.SaveChanges();

                            // Complete the transaction
                            //scope.Complete();

                            // Return the model
                            return Json(model, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            // Get a list of possible opponents
                            var possibleOpponents = context.ActiveUsers.Where(a => !a.IsInGame && a.UserID != me.ID).ToList();

                            if (possibleOpponents.Count() > 0)
                            {
                                // Generate a random indexer for the opponent
                                Random opponentID = new Random();
                                int opponentIndex = opponentID.Next(0, possibleOpponents.Count());

                                var opponentChosen = possibleOpponents[opponentIndex];

                                // Get the opponents details
                                var opponent = context.UserProfiles.Find(opponentChosen.UserID);

                                // Build a new game, together with both players
                                var newGame = new GameResult { GameDateTime = DateTime.UtcNow, IsActive = true };

                                var myGameRecord = new GameResultPlayer { BetAmount = 1.00m, GameResultID = newGame.ID, UserID = me.ID };
                                var opponentGameRecord = new GameResultPlayer { BetAmount = 1.00m, GameResultID = newGame.ID, UserID = opponent.ID };

                                context.GameResults.Add(newGame);
                                context.GameResultPlayers.Add(myGameRecord);
                                context.GameResultPlayers.Add(opponentGameRecord);

                                context.SaveChanges();

                                // Build the model
                                model = new GameViewModel
                                {
                                    GameResultID = newGame.ID,
                                    MyBetAmount = myGameRecord.BetAmount,
                                    MyName = me.FullName,
                                    MyUserID = me.ID,
                                    OpponentBetAmount = opponentGameRecord.BetAmount,
                                    OpponentID = opponent.ID,
                                    OpponentName = opponent.FullName,
                                    TotalMoney = me.Money
                                };

                                // Set both the current player and opponent to be in a game
                                var myActiveUser = context.ActiveUsers.Where(a => a.UserID == me.ID).FirstOrDefault();
                                myActiveUser.IsInGame = true;
                                myActiveUser.LastActionDate = DateTime.UtcNow;

                                var myActiveOpponent = context.ActiveUsers.Where(a => a.UserID == opponent.ID).FirstOrDefault();
                                myActiveUser.IsInGame = true;
                                myActiveOpponent.LastActionDate = DateTime.UtcNow;

                                context.SaveChanges();

                                // Complete the transaction
                                // scope.Complete();

                                return Json(model, JsonRequestBehavior.AllowGet);
                            }
                        }

                        // Nobody found, so try again in 5 seconds
                        Thread.Sleep(5000);
                    }
                }
            //}

            // We've been going 2 minutes now and noone is playing, so return an error
            return Json(false, JsonRequestBehavior.AllowGet);
           
        }

    }
}
