using HTECChampionsLeagueAPI.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HTECChampionsLeagueAPI.Models
{
    /// <summary>
    /// Helper class used to validate inserted matches and calculate group standings
    /// </summary>
    public class Group
    {
        #region Fields
        private HashSet<string> teams;
        private Dictionary<Matchday, List<Match>> matches;
        #endregion Fields

        #region Constructors
        public Group(GroupName groupName, IEnumerable<Match> playedMatches = null)
        {
            LeaguteTitle = string.Empty;
            Name = groupName;
            matches = new Dictionary<Matchday, List<Match>>();
            Standing = new List<Standing>();

            foreach (Matchday md in Enum.GetValues(typeof(Matchday)))
            {
                matches[md] = new List<Match>(2);
            }

            teams = new HashSet<string>(4);

            if (playedMatches != null && playedMatches.Count() > 0)
            {
                LeaguteTitle = playedMatches.ElementAt(0).LeagueTitle;

                foreach (Match m in playedMatches)
                {
                    UpdateTeamsWithinGroup(m);

                    TryAddMatch(m);
                }
            }
        }
        #endregion Constructors

        #region Properties
        [JsonProperty("group")]
        public string GroupName { get => Name.ToString(); }
        [JsonIgnore]
        public GroupName Name { get; }
        public string LeaguteTitle { get; set; }
        public List<Standing> Standing { get; set; }
        #endregion Properties

        #region Public methods
        public void TryAddMatch(Match m)
        {
            ValidateMatch(m);

            matches[m.Matchday].Add(m);

            UpdateStandings(m);
        }
        #endregion Public methods

        #region Private methods
        private void ValidateMatch(Match m)
        {
            if (Name != m.GroupName)
            {
                throw new ArgumentException($"Match {m} playedin group other than {GroupName}");
            }

            if ((teams.Count == 4 && (teams.Contains(m.AwayTeam) == false || teams.Contains(m.HomeTeam) == false))
                || (teams.Count == 3 && teams.Contains(m.AwayTeam) == false && teams.Contains(m.HomeTeam) == false))
            {
                throw new ArgumentException($"Group {GroupName} already has teams: {string.Join(", ", teams)} and can not add match {m}");
            }

            if (matches[m.Matchday].Count == 2)
            {
                throw new ArgumentException($"Both matches already played for {m.Matchday} matchday");
            }
            else if (matches[m.Matchday].Count == 1)
            {
                string homeTeam = matches[m.Matchday][0].HomeTeam;
                string awayTeam = matches[m.Matchday][0].AwayTeam;
                if (homeTeam.Equals(m.HomeTeam) || homeTeam.Equals(m.AwayTeam) ||
                    awayTeam.Equals(m.HomeTeam) || awayTeam.Equals(m.AwayTeam))
                {
                    throw new ArgumentException($"At least on of the teams in match: {m} played already in matchday {m.Matchday}.");
                }
            }

            UpdateTeamsWithinGroup(m);
        }

        private void UpdateTeamsWithinGroup(Match m)
        {
            if (teams.Contains(m.HomeTeam) == false)
            {
                teams.Add(m.HomeTeam);
            }

            if (teams.Contains(m.AwayTeam) == false)
            {
                teams.Add(m.AwayTeam);
            }
        }

        private void UpdateStandings(Match m)
        {
            Standing s = Standing.FirstOrDefault(x => x.Team.Equals(m.HomeTeam));

            if (s == null)
            {
                s = new Standing(m.HomeTeam);
                Standing.Add(s);
            }

            s.AddGame(true, m.HomeGoals, m.AwayGoals);

            s = Standing.FirstOrDefault(x => x.Team.Equals(m.AwayTeam));

            if (s == null)
            {
                s = new Standing(m.AwayTeam);
                Standing.Add(s);
            }

            s.AddGame(false, m.HomeGoals, m.AwayGoals);

            Standing = new List<Standing>(Standing.OrderByDescending(x => x.Points).ThenByDescending(x => x.Goals).ThenByDescending(x => x.GoalDifference));

            for (int i = 0; i != Standing.Count; i++)
            {
                Standing[i].Rank = i + 1;
            }
        }
        #endregion Private methods
    }
}
