using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperWinners.Core.Entities
{
    public class BaseEntity
    {
        public int ID { get; set; }
    }

    public class UserProfile : BaseEntity
    {
        [ForeignKey("BetBand")]
        public int BetBandID { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal Money { get; set; }

        public BetBand BetBand { get; set; }
    }

    public class BetBand : BaseEntity
    {
        public string BetBandName { get; set; }
        public decimal MinimumBet { get; set; }
        public decimal MaximumBet { get; set; }

        public ICollection<UserProfile> Users { get; set; }
    }

    public class GameResult :  BaseEntity
    {
        public DateTime GameDateTime { get; set; }
        public decimal? TotalBet { get; set; }
        public GameResultOutcome? ResultOutcome { get; set; }
        public bool IsActive { get; set; }

        public ICollection<GameResultPlayer> GameResultPlayers { get; set; }
    }

    public class GameResultPlayer : BaseEntity
    {
        [ForeignKey("GameResult")]
        public int GameResultID { get; set; }
        
        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        public decimal BetAmount { get; set; }
        public GameAction Action { get; set; }
        public GameResultOutcome ResultOutcome { get; set; }

        public UserProfile UserProfile { get; set; }
        public GameResult GameResult { get; set; }
    }

    public class ActiveUser : BaseEntity
    {
        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        public DateTime LastActionDate { get; set; }
        public bool IsInGame { get; set; }

        public UserProfile UserProfile { get; set; }
    }

    public enum GameResultOutcome
    {
        Player1Win = 1,
        Player2Win,
        Draw
    }

    public enum GameAction
    {
        Rock = 1,
        Paper,
        Scissors
    }
}
