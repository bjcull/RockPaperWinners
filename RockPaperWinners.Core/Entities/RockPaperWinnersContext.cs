using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RockPaperWinners.Core.Entities
{
    public class RockPaperWinnersContext : DbContext
    {
        public RockPaperWinnersContext() : base() { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<GameResult> GameResults { get; set; }
        public DbSet<GameResultPlayer> GameResultPlayers { get; set; }
        public DbSet<BetBand> BetBands { get; set; }
        public DbSet<ActiveUser> ActiveUsers { get; set; }
    }
}
