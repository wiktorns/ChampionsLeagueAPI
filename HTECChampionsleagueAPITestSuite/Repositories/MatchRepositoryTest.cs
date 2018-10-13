using HTECChampionsLeagueAPI.Contexts;
using HTECChampionsLeagueAPI.Enumerations;
using HTECChampionsLeagueAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using HTEC = HTECChampionsLeagueAPI.Models;

namespace HTECChampionsleagueAPITestSuite.Repositories
{
    public class MockMatchContext : MatchContext
    {
        DbSet<HTEC.Match> matchesDbSet;

        public MockMatchContext() : base(new DbContextOptions<MatchContext>()) { }

        public override int SaveChanges()
        {
            return 0;
        }

        public override DbSet<HTEC.Match> Matches
        {
            get { return matchesDbSet; }
            set { matchesDbSet = value;}
        }
    }

    public class MatchRepositoryTest
    {
        #region Private fields

        private MatchRepository repository;

        #endregion Private fields

        #region Constructors and Initializers

        public MatchRepositoryTest()
        {
            InitializeRepository();
        }

        private void InitializeRepository()
        {
            List<HTEC.Match> matches = InitializeTestMatches();

            DbSet<HTEC.Match> matchesDbSet = GetQueryableMockDbSet(matches);

            MockMatchContext moqContext = new MockMatchContext
            {
                Matches = matchesDbSet
            };

            repository = new MatchRepository(moqContext);
        }

        private static List<HTEC.Match> InitializeTestMatches()
        {
            List<HTEC.Match> matches = new List<HTEC.Match>();

            matches.Add(new HTEC.Match() { Id = 100, HomeTeam = "Inter", AwayTeam = "Milan", GroupName = GroupName.A, Matchday = Matchday.First, Score = "0:0", KickoffAt = new DateTime(2017, 1, 1) });
            matches.Add(new HTEC.Match() { Id = 101, HomeTeam = "Milan", AwayTeam = "Inter", GroupName = GroupName.C, Matchday = Matchday.Second, Score = "2:2", KickoffAt = new DateTime(2018, 1, 1) });

            return matches;
        }

        private static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
            dbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>((s) => sourceList.Remove(s));

            return dbSet.Object;
        }

        #endregion Constructors and Initializers

        #region Tests

        [Fact]
        public void ReturnsMatchFromDbIfExists()
        {
            HTEC.Match m = repository.GetMatch(100);

            Assert.NotNull(m);
        }

        [Fact]
        public void ReturnsNullIfMatchDoesNotExistInDb()
        {
            HTEC.Match m = repository.GetMatch(999);

            Assert.Null(m);
        }

        [Fact]
        public void DeletesMatchFromDbIfFound()
        {
            int matchCountBefore = repository.GetMatches(new HTEC.MatchFilter()).Count();

            HTEC.Match m = repository.DeleteMatch(100);

            int matchCountAfter = repository.GetMatches(new HTEC.MatchFilter()).Count();

            Assert.NotNull(m);
            Assert.True(matchCountBefore > matchCountAfter);
        }

        [Fact]
        public void ReturnsNullIfMatchToDeleteDoesNotExistInDb()
        {
            int matchCountBefore = repository.GetMatches(new HTEC.MatchFilter()).Count();

            HTEC.Match m = repository.DeleteMatch(999);

            int matchCountAfter = repository.GetMatches(new HTEC.MatchFilter()).Count();

            Assert.Null(m);
            Assert.True(matchCountBefore == matchCountAfter);
        }
        
        [Fact]
        public void FiltersMatchesProperlyWithGivenFilter()
        {
            InitializeRepository();

            HTEC.MatchFilter filter1 = new HTEC.MatchFilter() { Group = GroupName.A };
            HTEC.MatchFilter filter2 = new HTEC.MatchFilter() { From = new DateTime(2000, 1, 1) };
            HTEC.MatchFilter filter3 = new HTEC.MatchFilter() { Team = "Napoli" };
            HTEC.MatchFilter filter4 = new HTEC.MatchFilter() { To = new DateTime(2018, 12, 12) };
            HTEC.MatchFilter filter5 = new HTEC.MatchFilter() { From = new DateTime(2000, 1, 1), Group = GroupName.H };

            IEnumerable<HTEC.Match> matches1 = repository.GetMatches(filter1);
            Assert.True(1 == matches1.Count());

            IEnumerable<HTEC.Match> matches2 = repository.GetMatches(filter2);
            Assert.True(2 == matches2.Count());

            IEnumerable<HTEC.Match> matches3 = repository.GetMatches(filter3);
            Assert.True(0 == matches3.Count());

            IEnumerable<HTEC.Match> matches4 = repository.GetMatches(filter4);
            Assert.True(2 == matches4.Count());

            IEnumerable<HTEC.Match> matches5 = repository.GetMatches(filter5);
            Assert.True(0 == matches5.Count());
        }
        
        [Fact]
        public void ReturnsAllMatchesIfEmptyFilterSent()
        {
            InitializeRepository();

            IEnumerable<HTEC.Match> matches1 = repository.GetMatches(new HTEC.MatchFilter());

            Assert.True(matches1.Count() == 2);

            IEnumerable<HTEC.Match> matches2 = repository.GetMatches(null);

            Assert.True(matches2.Count() == 2);
        }

        [Fact]
        public void ThrowsExceptionWhenNoValidGroupRequested()
        {
            List<string> groups = new List<string>(2) { "ASD", "BLA" };

            Assert.Throws<ArgumentException>(() => { repository.GetGroupResults(groups); });
        }

        [Fact]
        public void ReturnsAllTheGroupsWhenNullParameterSent()
        {
            IEnumerable<HTEC.Group> groups = repository.GetGroupResults();

            Assert.NotNull(groups);
            Assert.True(groups.Count() == 8);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.B) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.C) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.D) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.E) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.F) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.G) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.H) != null);
        }

        [Fact]
        public void ReturnsOnlyRequestedAndValidGroups()
        {
            List<string> groupsNames = new List<string>(3) { "A", "DSAFDS", "C" };

            IEnumerable<HTEC.Group> groups = repository.GetGroupResults(groupsNames);

            Assert.NotNull(groups);
            Assert.True(groups.Count() == 2);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.B) == null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.C) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.D) == null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.E) == null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.F) == null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.G) == null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.H) == null);
        }

        [Fact]
        public void ThrowsExceptionWhenNoMatchesAreSent()
        {
            Assert.Throws<ArgumentException>(() => { repository.AddMatches(null); });
        }

        [Fact]
        public void ThrowsExceptionWhenNoValidMatchesAreSent()
        {
            HTEC.Match m = new HTEC.Match() { HomeTeam = "Inter", Score = "2:2" };

            Assert.Throws<ArgumentException>(() => { repository.AddMatches(new List<HTEC.Match>() { m }); });
        }

        [Fact]
        public void AddMatchesToRespectiveGroupsAndReturnsUpdatedState()
        {
            HTEC.Match m1 = new HTEC.Match() { GroupName = GroupName.A, HomeTeam = "Juventus", AwayTeam = "Napoli", Score = "2:2", Matchday = Matchday.First };
            HTEC.Match m2 = new HTEC.Match() { GroupName = GroupName.B, HomeTeam = "Torino", AwayTeam = "Parma", Score = "2:2", Matchday = Matchday.First };

            IEnumerable<HTEC.Group> groups = repository.AddMatches(new List<HTEC.Match>() { m1, m2 });

            Assert.NotNull(groups);
            Assert.True(groups.Count() == 8);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A).Standing.Count == 4);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.B) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.B).Standing.Count == 2);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.C) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.C).Standing.Count == 2);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.D) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.E) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.F) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.G) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.H) != null);
        }

        [Fact]
        public void ThrowsExceptionWhenNoScoreUpdatesAreSent()
        {
            Assert.Throws<ArgumentException>(() => { repository.EditMatches(null); });
        }

        [Fact]
        public void ThrowsExceptionWhenNoValidScoreUpdatesAreSent()
        {
            HTEC.ScoreUpdate su = new HTEC.ScoreUpdate() { MatchId = 100, UpdatedScore = "!:2" };

            Assert.Throws<ArgumentException>(() => { repository.EditMatches(new List<HTEC.ScoreUpdate>() { su }); });
        }

        [Fact]
        public void EditsMatchesScoresAndReturnsUpdatedGroupState()
        {
            InitializeRepository();

            HTEC.ScoreUpdate su = new HTEC.ScoreUpdate() { MatchId = 100, UpdatedScore = "5:0" };

            IEnumerable<HTEC.Group> groups = repository.EditMatches(new List<HTEC.ScoreUpdate>() { su });

            Assert.NotNull(groups);
            Assert.True(groups.Count() == 8);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A).Standing.Count == 2);
            Assert.Equal("Inter", groups.FirstOrDefault(x => x.Name == GroupName.A).Standing[0].Team);
            Assert.Equal(5, groups.FirstOrDefault(x => x.Name == GroupName.A).Standing[0].Goals);
            Assert.Equal("Milan", groups.FirstOrDefault(x => x.Name == GroupName.A).Standing[1].Team);
            Assert.Equal(0, groups.FirstOrDefault(x => x.Name == GroupName.A).Standing[1].Goals);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.A).Standing.Count == 2);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.B) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.C) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.D) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.E) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.F) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.G) != null);
            Assert.True(groups.FirstOrDefault(x => x.Name == GroupName.H) != null);
        }

        #endregion Tests
    }
}
