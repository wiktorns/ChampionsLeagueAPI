using System;

namespace HTECChampionsLeagueAPI.Models
{
    /// <summary>
    /// Helper class which models group standing for one team
    /// </summary>
    public class Standing
    {
        #region Constructors
        public Standing(string teamName)
        {
            Team = teamName;
        }
        #endregion Constructors

        #region Properties
        public string Team { get; }
        public int Points { get; internal set; }
        public int Goals { get; internal set; }
        public int GoalsAgainst { get; internal set; }
        public int GoalDifference { get; internal set; }
        public int Rank { get; set; }
        public int PlayedGames { get; internal set; }
        public int Win { get; internal set; }
        public int Lose { get; internal set; }
        public int Draw { get; internal set; }
        #endregion Properties

        #region Public methods
        public void AddGame(bool isHomeTeam, int homeGoals, int awayGoals)
        {
            if (homeGoals < 0) throw new ArgumentException($"Number {nameof(homeGoals)} must not be a negative number");
            if (awayGoals < 0) throw new ArgumentException($"Number {nameof(awayGoals)} must not be a negative number");

            if (isHomeTeam)
            {
                AddHomeTeamGame(homeGoals, awayGoals);
            }
            else
            {
                AddAwayTeamGame(homeGoals, awayGoals);
            }
        }

        #endregion Public methods

        #region Private methods
        private void AddHomeTeamGame(int homeGoals, int awayGoals)
        {
            PlayedGames++;

            if (homeGoals > awayGoals)
            {
                Win++;
            }
            else if (homeGoals == awayGoals)
            {
                Draw++;
            }
            else
            {
                Lose++;
            }

            Points = Win * 3 + Draw;
            Goals += homeGoals;
            GoalsAgainst += awayGoals;
            GoalDifference = Goals - GoalsAgainst;
        }

        private void AddAwayTeamGame(int homeGoals, int awayGoals)
        {
            PlayedGames++;

            if (homeGoals > awayGoals)
            {
                Lose++;
            }
            else if (homeGoals == awayGoals)
            {
                Draw++;
            }
            else
            {
                Win++;
            }

            Points = Win * 3 + Draw;
            Goals += awayGoals;
            GoalsAgainst += homeGoals;
            GoalDifference = Goals - GoalsAgainst;
        }
        #endregion Private methods
    }
}