using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockPaperWinners.Core.Entities;

namespace RockPaperWinners.Core.Initializers
{
    public class RockPaperWinnersDbInitializer : DropCreateDatabaseIfModelChanges<RockPaperWinnersContext>
    {
        protected override void Seed(RockPaperWinnersContext context)
        {
            var betbands = new List<BetBand>
            {
                new BetBand { ID = 1, BetBandName = "Casual", MinimumBet = 1.00M, MaximumBet = 5.00M },
                new BetBand { ID = 2, BetBandName = "Regular", MinimumBet = 5.00M, MaximumBet = 10.00M }
            };
            betbands.ForEach(s => context.BetBands.Add(s));
            context.SaveChanges();
        }
    }
}
