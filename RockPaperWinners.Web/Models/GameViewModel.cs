using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RockPaperWinners.Core.Entities;

namespace RockPaperWinners.Web.Models
{
    public class GameViewModel
    {
        // Game reference
        public int GameResultID { get; set; }

        // My details
        public int MyUserID { get; set; }
        public string MyName { get; set; }
        public decimal MyBetAmount { get; set; }
        public decimal TotalMoney { get; set; }
        
        // Opponent details
        public int OpponentID { get; set; }
        public string OpponentName { get; set; }
        public decimal OpponentBetAmount { get; set; }
    }
}