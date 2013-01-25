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
            return View();
        }

        [HttpGet]
        public JsonResult FindGame()
        {
            // Model to return if game found
            var model = new GameViewModel();

            // Initialise the context
            using (var context = new RockPaperWinnersContext())
            {
                // Set the current user as available
                if (!context.ActiveUsers.Where(a => a.UserID == WebSecurity.CurrentUserId).Any())
                {
                    context.ActiveUsers.Add(new ActiveUser { IsInGame = false, LastActionDate = DateTime.UtcNow, UserID = WebSecurity.CurrentUserId });
                    context.SaveChanges();
                }

                // Record the current datetimeutc, set variable to be a time when we will give up trying to find someone
                DateTime waitUntilTime = DateTime.UtcNow.AddSeconds(10);

                // BEGIN TRAN

                // While not timed out, check the game result entities to find any active game results with my id
                while (DateTime.UtcNow <= waitUntilTime)
                {
                    // Get the current user
                    var me = context.UserProfiles.Find(WebSecurity.CurrentUserId);

                    // Look to see if there are any active games for the current user
                    var activeGame = (from gp in context.GameResultPlayers where gp.GameResult.IsActive && gp.UserID == me.ID select gp).FirstOrDefault();

                    if (activeGame != null)
                    {
                        var game = context.GameResults.Include("GameResultPlayers").Where(g => g.ID == activeGame.GameResultID).FirstOrDefault();

                        // Find the opponent
                        var opponentGame = game.GameResultPlayers.Where(gp => gp.UserID != me.ID).FirstOrDefault();
                        var opponent = context.UserProfiles.Find(opponentGame.UserID);
                        var myGame = game.GameResultPlayers.Where(gp => gp.UserID == me.ID).FirstOrDefault();

                        // Build the model
                        // TODO: Generate random bet amounts
                        model = new GameViewModel
                        {
                            GameResultID = game.ID,
                            MyUserID = me.ID,
                            OpponentID = opponentGame.UserID,
                            MyBetAmount = myGame.BetAmount,
                            OpponentBetAmount = opponentGame.BetAmount,
                            MyName = me.FullName,
                            OpponentName = context.UserProfiles.Find(opponentGame.UserID).FullName,
                            TotalMoney = me.Money,
                            OpponentImageSource = GravatarHelper.GravatarHelper.CreateGravatarUrl(opponent.Email, 128, "identicon", null, null, null)
                        };

                        // Set the current user to be in a game
                        var activeUser = context.ActiveUsers.Where(a => a.UserID == me.ID).FirstOrDefault();
                        activeUser.IsInGame = true;
                        activeUser.LastActionDate = DateTime.UtcNow;

                        context.SaveChanges();

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
                                TotalMoney = me.Money,
                                OpponentImageSource = GravatarHelper.GravatarHelper.CreateGravatarUrl(opponent.Email, 128, "identicon", null, null, null)
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

                    // Nobody found, so try again in 1 seconds
                    Thread.Sleep(1000);
                }


                // We've been going 2 minutes now and noone is playing, so remove the user from looking for a game and return false
                var dormantUser = context.ActiveUsers.Where(u => u.UserID == WebSecurity.CurrentUserId).FirstOrDefault();
                context.ActiveUsers.Remove(dormantUser);
                context.SaveChanges();

                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult SubmitAction(int gameResultID, int playerID, string gameActionString)
        {
            GameResultPlayer gameResult = null;

            using (RockPaperWinnersContext context = new RockPaperWinnersContext())
            {
                GameAction gameAction;

                gameResult = context.GameResultPlayers.Where(g => g.GameResultID == gameResultID && g.UserID == playerID).FirstOrDefault();

                // If there is already an action, just end
                if (gameResult.Action.HasValue)
                {
                    return Json("This action has already been processed", JsonRequestBehavior.AllowGet);
                }

                // If the action supplied is invalid, just end
                if (!Enum.TryParse(gameActionString, true, out gameAction))
                {
                    return Json("Invalid action supplied", JsonRequestBehavior.AllowGet);
                }

                gameResult.Action = gameAction;

                context.SaveChanges();
            }

            // Record the current datetimeutc, set variable to be a time when we will give up trying to find someone
            DateTime waitUntilTime = DateTime.UtcNow.AddSeconds(60);

            // BEGIN TRAN

            // While not timed out, check the game result entities to find any active game results with my id
            while (DateTime.UtcNow <= waitUntilTime)
            {
                using (RockPaperWinnersContext context = new RockPaperWinnersContext())
                {
                    // Get the opponents action
                    var opponentGameResult = context.GameResultPlayers.Where(g => g.GameResultID == gameResultID && g.UserID != playerID).FirstOrDefault();

                    if (opponentGameResult != null && opponentGameResult.Action.HasValue && !opponentGameResult.ResultOutcome.HasValue)
                    {
                        int winner = (3 + (int)gameResult.Action.Value - (int)opponentGameResult.Action.Value) % 3;

                        switch (winner)
                        {
                            case 1:
                                gameResult.ResultOutcome = GamePlayerResultOutcome.IWin;
                                opponentGameResult.ResultOutcome = GamePlayerResultOutcome.OpponentWin;
                                break;

                            case 2:
                                gameResult.ResultOutcome = GamePlayerResultOutcome.OpponentWin;
                                opponentGameResult.ResultOutcome = GamePlayerResultOutcome.IWin;
                                break;

                            case 0:
                                gameResult.ResultOutcome = GamePlayerResultOutcome.Draw;
                                opponentGameResult.ResultOutcome = GamePlayerResultOutcome.Draw;
                                break;
                        }

                        var game = context.GameResults.Find(gameResultID);
                        game.IsActive = false;

                        var activeUser = context.ActiveUsers.Where(u => u.UserID == playerID).FirstOrDefault();
                        if (activeUser != null)
                        {
                            context.ActiveUsers.Remove(activeUser);
                        }

                        context.SaveChanges();

                        var result = new GameResultModel()
                        {
                            GameResult = (int)gameResult.ResultOutcome
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (opponentGameResult != null && opponentGameResult.Action.HasValue && opponentGameResult.ResultOutcome.HasValue)
                    {
                        var activeUser = context.ActiveUsers.Where(u => u.UserID == playerID).FirstOrDefault();
                        if (activeUser != null)
                        {
                            context.ActiveUsers.Remove(activeUser);
                        }

                        context.SaveChanges();

                        var result = new GameResultModel()
                        {
                            GameResult = (int)gameResult.ResultOutcome
                        };

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (opponentGameResult != null && opponentGameResult.Action.HasValue)
                    {
                        return Json("Has Opponent Action, but result outcome is screwed.", JsonRequestBehavior.AllowGet);
                    }
                    else if (opponentGameResult == null)
                    {
                        return Json("opponent game result is null???", JsonRequestBehavior.AllowGet);
                    }
                }
                // Nobody found, so try again in 1 seconds
                Thread.Sleep(1000);
            }

            // Something went wrong, there's no opponent result
            return Json("Game Timed Out", JsonRequestBehavior.AllowGet);
        }


    }
}

