namespace HTECChampionsLeagueAPI.Models
{
    /// <summary>
    /// Helper class which modelsmatch score update
    /// </summary>
    public class ScoreUpdate
    {
        #region Constructors
        public ScoreUpdate()
        {
            UpdatedScore = string.Empty;
        }
        #endregion Constructors

        #region Properties
        public int MatchId { get; set; }
        public string UpdatedScore { get; set; }
        #endregion Properties

        #region Public methods
        public bool IsValid()
        {
            if (MatchId <= 0) return false;

            if (string.IsNullOrEmpty(UpdatedScore)) return false;

            string[] check = UpdatedScore.Split(':');

            if (check.Length != 2) return false;

            if (int.TryParse(check[0], out int homeGoals) == false || homeGoals < 0) return false;

            if (int.TryParse(check[1], out int awayGoals) == false || awayGoals < 0) return false;

            return true;
        }

        public override string ToString()
        {
            return $"Match id: {MatchId} - Score: {UpdatedScore}";
        }
        #endregion Public methods
    }
}
