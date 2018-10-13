using HTECChampionsLeagueAPI.Enumerations;
using HTECChampionsLeagueAPI.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace HTECChampionsleagueAPITestSuite.Models.BusinessLogic
{
    public class GroupTest
    {
        [Fact]
        public void NotPossibleToAddMatchFromAnotherGroup()
        {
            Group g = new Group(GroupName.B);

            Match m = new Match() { GroupName = GroupName.A };

            Assert.Throws<ArgumentException>(() => { g.TryAddMatch(m); });
        }

        [Fact]
        public void NotPossibleToAddMatchesFromMoreThanFourDifferentTeamsToGroup()
        {
            Group g1 = new Group(GroupName.B);

            Match m1 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Inter", HomeTeam = "Milan", Matchday = Matchday.First };
            Match m2 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Juventus", HomeTeam = "Napoli", Matchday = Matchday.First };
            Match m3 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Inter", HomeTeam = "Torino", Matchday = Matchday.Second };

            g1.TryAddMatch(m1);
            g1.TryAddMatch(m2);

            Assert.Throws<ArgumentException>(() => { g1.TryAddMatch(m3); });

            Group g2 = new Group(GroupName.C);

            Match m4 = new Match() { GroupName = GroupName.C, Score = "0:0", AwayTeam = "Chelsea", HomeTeam = "Manchester", Matchday = Matchday.First };
            Match m5 = new Match() { GroupName = GroupName.C, Score = "0:0", AwayTeam = "West Ham", HomeTeam = "Chelsea", Matchday = Matchday.Second };
            Match m6 = new Match() { GroupName = GroupName.C, Score = "0:0", AwayTeam = "Aston Villa", HomeTeam = "Arsenal", Matchday = Matchday.Second };

            g2.TryAddMatch(m4);
            g2.TryAddMatch(m5);

            Assert.Throws<ArgumentException>(() => { g2.TryAddMatch(m6); });
        }

        [Fact]
        public void NotPossibleToAddMoreThanTwoMatchesOnMatchdayToGroup()
        {
            Group g1 = new Group(GroupName.B);

            Match m1 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Inter", HomeTeam = "Milan", Matchday = Matchday.First };
            Match m2 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Juventus", HomeTeam = "Napoli", Matchday = Matchday.First };
            Match m3 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Inter", HomeTeam = "Napoli", Matchday = Matchday.First };

            g1.TryAddMatch(m1);
            g1.TryAddMatch(m2);

            Assert.Throws<ArgumentException>(() => { g1.TryAddMatch(m3); });
        }

        [Fact]
        public void NotPossibleToAddSameTeamTwiceOnMatchdayToGroup()
        {
            Group g1 = new Group(GroupName.B);

            Match m1 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Inter", HomeTeam = "Milan", Matchday = Matchday.First };
            Match m2 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Juventus", HomeTeam = "Inter", Matchday = Matchday.First };
            
            g1.TryAddMatch(m1);

            Assert.Throws<ArgumentException>(() => { g1.TryAddMatch(m2); });
        }

        [Fact]
        public void UpdatesStandingAfterEveryAddedMatch()
        {
            Match m1 = new Match() { GroupName = GroupName.B, Score = "2:0", AwayTeam = "Inter", HomeTeam = "Milan", Matchday = Matchday.First };
            Match m2 = new Match() { GroupName = GroupName.B, Score = "3:0", AwayTeam = "Juventus", HomeTeam = "Napoli", Matchday = Matchday.First };

            Group g1 = new Group(GroupName.B, new List<Match>(2) { m1, m2 });

            Assert.True(g1.Standing[0].Rank == 1 && g1.Standing[0].Team.Equals("Napoli"));
            Assert.True(g1.Standing[1].Rank == 2 && g1.Standing[1].Team.Equals("Milan"));
            Assert.True(g1.Standing[2].Rank == 3 && g1.Standing[2].Team.Equals("Inter"));
            Assert.True(g1.Standing[3].Rank == 4 && g1.Standing[3].Team.Equals("Juventus"));

            Match m3 = new Match() { GroupName = GroupName.B, Score = "0:0", AwayTeam = "Juventus", HomeTeam = "Milan", Matchday = Matchday.Second };

            g1.TryAddMatch(m3);

            Assert.True(g1.Standing[0].Rank == 1 && g1.Standing[0].Team.Equals("Milan"));
            Assert.True(g1.Standing[1].Rank == 2 && g1.Standing[1].Team.Equals("Napoli"));
            Assert.True(g1.Standing[2].Rank == 3 && g1.Standing[2].Team.Equals("Juventus"));
            Assert.True(g1.Standing[3].Rank == 4 && g1.Standing[3].Team.Equals("Inter"));

            Match m4 = new Match() { GroupName = GroupName.B, Score = "5:0", AwayTeam = "Napoli", HomeTeam = "Inter", Matchday = Matchday.Second };

            g1.TryAddMatch(m4);

            Assert.True(g1.Standing[0].Rank == 1 && g1.Standing[0].Team.Equals("Milan"));
            Assert.True(g1.Standing[1].Rank == 2 && g1.Standing[1].Team.Equals("Inter"));
            Assert.True(g1.Standing[2].Rank == 3 && g1.Standing[2].Team.Equals("Napoli"));
            Assert.True(g1.Standing[3].Rank == 4 && g1.Standing[3].Team.Equals("Juventus"));
        }
    }
}
