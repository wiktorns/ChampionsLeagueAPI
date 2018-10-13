using HTECChampionsLeagueAPI.Enumerations;
using System;

namespace HTECChampionsLeagueAPI.Models
{
    /// <summary>
    /// Helper class models filter used to filter out matches
    /// </summary>
    public class MatchFilter
    {
        #region Properties
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public GroupName? Group { get; set; }
        public string Team { get; set; }
        #endregion Properties

        #region Public methods
        public bool IsValid()
        {
            if (From.HasValue && To.HasValue
                && From.Value > To.Value)
            {
                return false;
            }

            return true;
        }
        #endregion Public methods
    }
}