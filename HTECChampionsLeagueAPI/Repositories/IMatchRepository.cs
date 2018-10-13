using HTECChampionsLeagueAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTECChampionsLeagueAPI.Repositories
{
    public interface IMatchRepository
    {
        Match GetMatch(long id);
        Task<Match> GetMatchAsync(long id);

        Match DeleteMatch(long id);

        IEnumerable<Group> GetGroupResults(IEnumerable<string> groups = null);

        IEnumerable<Match> GetMatches(MatchFilter filter);

        IEnumerable<Group> AddMatches(IEnumerable<Match> matches);
        IEnumerable<Group> EditMatches(IEnumerable<ScoreUpdate> matches);    
    }
}
