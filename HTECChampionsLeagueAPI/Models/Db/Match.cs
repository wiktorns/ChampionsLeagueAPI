using HTECChampionsLeagueAPI.Enumerations;
using Newtonsoft.Json;
using System;

namespace HTECChampionsLeagueAPI.Models
{
    /// <summary>
    /// Class models match entity stored within the data base
    /// </summary>
    public class Match
    {
        #region Constructors

        public Match()
        {
            LeagueTitle = string.Empty;
            HomeTeam = string.Empty;
            AwayTeam = string.Empty;
            Score = string.Empty;
            KickoffAt = new DateTime(1970, 1, 1);
        }

        #endregion Constructors

        #region Properties
        public long Id { get; set; }
        public string LeagueTitle { get; set; }
        public Matchday Matchday { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime KickoffAt { get; set; }
        public string Score { get; set; }

        public string Group
        {
            get => GroupName.ToString();
            set => GroupName = Enum.Parse<GroupName>(value);
        }

        [JsonIgnore]
        public GroupName GroupName { get; set; }

        [JsonIgnore]
        public byte AwayGoals
        {
            get
            {
                return IsScoreValid() ? byte.Parse(Score.Split(':')[1]) : (byte)0;
            }
        }

        [JsonIgnore]
        public byte HomeGoals
        {
            get
            {
                return IsScoreValid() ? byte.Parse(Score.Split(':')[0]) : (byte)0;
            }
        }
        #endregion Properties

        #region Public methods
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(HomeTeam))
            {
                return false;
            }

            if (string.IsNullOrEmpty(AwayTeam))
            {
                return false;
            }

            if (AwayTeam.Equals(HomeTeam, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (IsScoreValid() == false)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return $"{GroupName} - {HomeTeam}:{AwayTeam} - {Score}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Match)
            {
                return GetHashCode() == ((Match)obj).GetHashCode();
            }

            return false;
        }

        public override string ToString()
        {
            return $"{LeagueTitle} - {GroupName} -  {HomeTeam} {Score} {AwayTeam} - Matchday:{Matchday} Kickoff:{KickoffAt}";
        }
        #endregion Public methods

        #region Private methods
        private bool IsScoreValid()
        {
            string[] goals = Score.Split(':');

            if(goals.Length != 2)
            {
                return false;
            }

            return int.TryParse(goals[0], out int home) && home >= 0 
                && int.TryParse(goals[1], out int away) && away >= 0;
        }
        #endregion Private methods
    }
}
