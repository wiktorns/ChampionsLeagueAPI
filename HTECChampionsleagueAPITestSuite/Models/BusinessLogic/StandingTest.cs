using HTECChampionsLeagueAPI.Models;
using System;
using Xunit;

namespace HTECChampionsleagueAPITestSuite.Models.BusinessLogic
{
    public class StandingTest
    {
        private readonly Standing testStanding;

        public StandingTest()
        {
            testStanding = new Standing("Inter");
        }

        [Fact]
        public void IsNotValidIfScoreNotSet()
        {
            ScoreUpdate su = new ScoreUpdate() { UpdatedScore = null };

            Assert.False(su.IsValid());
        }

        [Theory]
        [InlineData(true, -1, 0)]
        [InlineData(true, 5, -4)]
        [InlineData(false, -3, 8)]
        [InlineData(false, 0,-2)]
        public void DoesNotAcceptNegativeNumberOfGoals(bool isHomeGame, int homeGoals, int awayGoals)
        {
            Standing s = new Standing("test");

            Assert.Throws<ArgumentException>(() => { s.AddGame(isHomeGame, homeGoals, awayGoals); });
        }

        [Theory]
        //          home?  hg ag pm p  w  d  l  gf  ga gd
        [InlineData(true,  5, 0, 1, 3, 1, 0, 0, 5, 0, 5)]
        [InlineData(true,  3, 3, 1, 1, 0, 1, 0, 3, 3, 0)]
        [InlineData(true,  0, 1, 1, 0, 0, 0, 1, 0, 1, -1)]
        [InlineData(false, 2, 1, 1, 0, 0, 0, 1, 1, 2, -1)]
        [InlineData(false, 2, 2, 1, 1, 0, 1, 0, 2, 2, 0)]
        [InlineData(false, 0, 1, 1, 3, 1, 0, 0, 1, 0, 1)]
        public void UpdatesStateCorrectlyAfterGame(bool isHomeGame, int homeGoals, int awayGoals, int expPlayedGames,
            int expPoints, int expWins, int expDraws, int expLoses, int expGoalsFor, int expGoalsAg, int expGoalDif)
        {
            testStanding.AddGame(isHomeGame, homeGoals, awayGoals);

            Assert.True(testStanding.PlayedGames == expPlayedGames);
            Assert.True(testStanding.Points == expPoints);
            Assert.True(testStanding.Win == expWins);
            Assert.True(testStanding.Draw == expDraws);
            Assert.True(testStanding.Lose == expLoses);
            Assert.True(testStanding.Goals == expGoalsFor);
            Assert.True(testStanding.GoalsAgainst == expGoalsAg);
            Assert.True(testStanding.GoalDifference == expGoalDif);
        }
    }
}
