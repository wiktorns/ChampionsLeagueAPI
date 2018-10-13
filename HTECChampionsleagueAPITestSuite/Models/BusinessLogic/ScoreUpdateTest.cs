using HTECChampionsLeagueAPI.Models;
using Xunit;

namespace HTECChampionsleagueAPITestSuite.Models.BusinessLogic
{
    public class ScoreUpdateTest
    {
        [Fact]
        public void IsNotValidIfScoreNotSet()
        {
            ScoreUpdate su = new ScoreUpdate() { MatchId = 101 };

            Assert.False(su.IsValid());
        }

        [Fact]
        public void IsNotValidIfMatchIdNotSet()
        {
            ScoreUpdate su = new ScoreUpdate() { UpdatedScore = "1:1" };

            Assert.False(su.IsValid());
        }

        [Theory]
        [InlineData("1::1")]
        [InlineData("1:1:")]
        [InlineData(":1:")]
        [InlineData(":1:1")]
        public void IsNotValidIfGivenMoreThanOneColon(string score)
        {
            ScoreUpdate su = new ScoreUpdate() { MatchId = 101, UpdatedScore = score };

            Assert.False(su.IsValid());
        }

        [Theory]
        [InlineData("a:1")]
        [InlineData("1:a")]
        [InlineData("-1:1")]
        [InlineData("0:-1")]
        public void IsNotValidIfGivenOtherThanNonNegativeNumbers(string score)
        {
            ScoreUpdate su = new ScoreUpdate() { MatchId = 101, UpdatedScore = score };

            Assert.False(su.IsValid());
        }

        [Theory]
        [InlineData("1:1")]
        [InlineData("5:0")]
        [InlineData("0:0")]
        [InlineData("1:3")]
        public void IsValidWhenGivenParsableNonNegativeNumbers(string score)
        {
            ScoreUpdate su = new ScoreUpdate() { MatchId = 101, UpdatedScore = score };

            Assert.True(su.IsValid());
        }
    }
}
