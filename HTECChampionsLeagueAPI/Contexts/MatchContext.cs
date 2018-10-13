using HTECChampionsLeagueAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HTECChampionsLeagueAPI.Contexts
{
    public class MatchContext : DbContext
    {
        public MatchContext(DbContextOptions<MatchContext> options)
            : base(options) { }

        public virtual DbSet<Match> Matches { get; set; }
    }
}
