using HTECChampionsLeagueAPI.Models;
using Xunit;

namespace HTECChampionsleagueAPITestSuite.Models.Db
{
    public class MatchTest
    {
        [Theory]
        [InlineData(null, "Inter")]
        [InlineData("", "Milan")]
        [InlineData("Juventus", null)]
        [InlineData("Napoli", "")]
        public void IsNotValidIfHasNoSetTeams(string homeTeam, string awayTeam)
        {
            Match m = new Match() { HomeTeam = homeTeam, AwayTeam = awayTeam };

            Assert.False(m.IsValid());
        }

        [Theory]
        [InlineData("Milan", "Inter", "asd")]
        [InlineData("Inter", "Milan", "1:2s")]
        [InlineData("Juventus", "Napoli", "1::3")]
        [InlineData("Napoli", "Juventus", "-1:4")]
        public void IsNotValidIfHasNoParsableScore(string homeTeam, string awayTeam, string score)
        {
            Match m = new Match() { HomeTeam = homeTeam, AwayTeam = awayTeam, Score = score };

            Assert.False(m.IsValid());
        }

        [Fact]
        public void IsValidIfHasBothTeamsAndScoreSet()
        {
            Match m = new Match() { HomeTeam = "Inter", AwayTeam = "Juventus", Score= "1:0" };

            Assert.True(m.IsValid());
        }
    }
}
