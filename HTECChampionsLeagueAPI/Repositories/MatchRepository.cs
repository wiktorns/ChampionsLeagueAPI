using HTECChampionsLeagueAPI.Contexts;
using HTECChampionsLeagueAPI.Enumerations;
using HTECChampionsLeagueAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HTECChampionsLeagueAPI.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        #region Fields

        private readonly MatchContext matchContext;

        #endregion Fields

        #region Constructors

        public MatchRepository(MatchContext context)
        {
            matchContext = context;
        }

        #endregion Constructors

        #region Public methods

        public Match GetMatch(long id)
        {
            return matchContext.Matches.FirstOrDefault(x => x.Id == id);
        }

        public Task<Match> GetMatchAsync(long id)
        {
            return matchContext.FindAsync<Match>(id);
        }

        public Match DeleteMatch(long id)
        {
            Match deletedMatch = matchContext.Matches.FirstOrDefault(x => x.Id == id);

            if (deletedMatch != null)
            {
                matchContext.Matches.Remove(deletedMatch);
                matchContext.SaveChanges();
            }

            return deletedMatch;
        }

        public IEnumerable<Group> GetGroupResults(IEnumerable<string> groups = null)
        {
            HashSet<GroupName> groupsSet = null;
            if (groups != null && groups.Count() > 0)
            {
                groupsSet = new HashSet<GroupName>();

                foreach (string group in groups)
                {
                    if (Enum.TryParse(group, out GroupName g) && groupsSet.Contains(g) == false)
                    {
                        groupsSet.Add(g);
                    }
                }

                if (groupsSet.Count == 0)
                {
                    throw new ArgumentException("No valid groups were sent.");
                }
            }

            return FetchCurrentGroupStates(groupsSet).Values;
        }

        public IEnumerable<Match> GetMatches(MatchFilter filter)
        {
            if (filter == null) filter = new MatchFilter();

            if (filter.IsValid() == false)
            {
                throw new ArgumentException($"In filter instance {nameof(filter.From)} must be before {nameof(filter.To)}");
            }

            IEnumerable<Match> result = matchContext.Matches;

            if (filter.From.HasValue)
            {
                result = result.Where(x => x.KickoffAt >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                result = result.Where(x => x.KickoffAt <= filter.To.Value);
            }

            if (filter.Group.HasValue)
            {
                result = result.Where(x => x.GroupName == filter.Group.Value);
            }

            if (string.IsNullOrEmpty(filter.Team) == false)
            {
                result = result.Where(x => x.HomeTeam.Equals(filter.Team, StringComparison.OrdinalIgnoreCase)
                || x.AwayTeam.Equals(filter.Team, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }

        public IEnumerable<Group> AddMatches(IEnumerable<Match> matches)
        {
            if (matches == null)
            {
                throw new ArgumentException($"Matches not sent", nameof(matches));
            }

            foreach (Match m in matches)
            {
                if (m.IsValid() == false) throw new ArgumentException($"Match instance is not valid: {m.ToString()}", nameof(matches));
            }

            Dictionary<GroupName, List<Match>> matchesByGroupToInsert = GroupMatchesByGroupName(matches);

            Dictionary<GroupName, Group> currentGroupState = FetchCurrentGroupStates();

            foreach (GroupName g in matchesByGroupToInsert.Keys)
            {
                foreach (Match m in matchesByGroupToInsert[g])
                {
                    currentGroupState[g].TryAddMatch(m);
                }
            }

            matchContext.Matches.AddRange(matches);
            matchContext.SaveChanges();

            return currentGroupState.Values;
        }

        public IEnumerable<Group> EditMatches(IEnumerable<ScoreUpdate> updates)
        {
            if (updates == null)
            {
                throw new ArgumentException($"Updates not sent", nameof(updates));
            }

            foreach (ScoreUpdate su in updates)
            {
                if (su.IsValid() == false) throw new ArgumentException($"Score update instance is not valid: {su.ToString()}", nameof(updates));
            }

            foreach (ScoreUpdate su in updates)
            {
                Match existingMatch = matchContext.Matches.FirstOrDefault(x => x.Id == su.MatchId);

                if (existingMatch == null) continue;

                existingMatch.Score = su.UpdatedScore;

                matchContext.Matches.Update(existingMatch);
            }

            matchContext.SaveChanges();

            Dictionary<GroupName, Group> currentGroupState = FetchCurrentGroupStates();

            return currentGroupState.Values;
        }

        #endregion Public methods

        #region Private methods

        private Dictionary<GroupName, Group> FetchCurrentGroupStates(IEnumerable<GroupName> groups = null)
        {
            Array groupsToFetch = groups == null ? Enum.GetValues(typeof(GroupName)) : groups.ToArray();

            Dictionary<GroupName, Group> state = new Dictionary<GroupName, Group>();

            foreach (GroupName g in groupsToFetch)
            {
                IEnumerable<Match> storedMatchesOfTheGroup = matchContext.Matches.Where(x => x.GroupName == g);

                Group existingGroup = new Group(g, storedMatchesOfTheGroup);

                state[g] = existingGroup;
            }

            return state;
        }

        private static Dictionary<GroupName, List<Match>> GroupMatchesByGroupName(IEnumerable<Match> matches)
        {
            Dictionary<GroupName, List<Match>> matchesByGroupToInsert = new Dictionary<GroupName, List<Match>>();

            foreach (Match m in matches)
            {
                if (matchesByGroupToInsert.TryGetValue(m.GroupName, out List<Match> groupMatches) == false)
                {
                    groupMatches = new List<Match>();
                    matchesByGroupToInsert[m.GroupName] = groupMatches;
                }

                groupMatches.Add(m);
            }

            return matchesByGroupToInsert;
        }

        #endregion Private methods
    }
}
