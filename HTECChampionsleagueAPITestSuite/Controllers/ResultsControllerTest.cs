using HTECChampionsLeagueAPI.Controllers;
using HTECChampionsLeagueAPI.Enumerations;
using HTECChampionsLeagueAPI.Models;
using HTECChampionsLeagueAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using HTEC = HTECChampionsLeagueAPI.Models;

namespace HTECChampionsleagueAPITestSuite.Controllers
{
    public class ResultsControllerTest
    {
        #region Fields
        private ResultsController controller;
        private static readonly string invalidGroupsParam = "asd,bsd";
        private static readonly string validGroupsParam = "A,C";
        private static readonly int invalidMatchParam = 202;
        private static readonly int validMatchParam = 101;
        private static readonly IEnumerable<string> invalidGroupsList = new List<string>(2){ "asd", "bsd" };
        private static readonly IEnumerable<string> validGroupsList = new List<string>(2) { "A", "C" };
        private static readonly IEnumerable<ScoreUpdate> validUpdates = new List<ScoreUpdate>(1) { new ScoreUpdate() { MatchId = 101, UpdatedScore = "1:1" } };
        private static readonly IEnumerable<ScoreUpdate> invalidUpdates = new List<ScoreUpdate>(1) { new ScoreUpdate() { MatchId = 101, UpdatedScore = "asd:1" } };
        private static readonly IEnumerable<HTEC.Match> invalidMatches = new List<HTEC.Match>(1) { new HTEC.Match() { GroupName = GroupName.A, AwayTeam = "Milan", Score = "3:0", Matchday = Matchday.First } };
        private static readonly IEnumerable<HTEC.Match> validMatches = new List<HTEC.Match>(1) { new HTEC.Match() { GroupName = GroupName.A, HomeTeam = "Inter", AwayTeam = "Milan", Score = "3:0", Matchday = Matchday.First } };
        private static readonly MatchFilter validFilter = new MatchFilter();
        private static readonly MatchFilter invalidFilter = new MatchFilter() { From = new DateTime(2019), To = new DateTime(2018) };
        #endregion Privatefields

        #region Constructors and Initializers

        public ResultsControllerTest()
        {
            InitializeController();
        }

        private void InitializeController()
        {
            Mock<IMatchRepository> repositoryMock = new Mock<IMatchRepository>();

            repositoryMock.Setup(x => x.GetGroupResults(invalidGroupsList)).Throws<ArgumentException>();

            InitializeGroups(repositoryMock);

            controller = new ResultsController(repositoryMock.Object);
        }

        private void InitializeGroups(Mock<IMatchRepository> repositoryMock)
        {
            HTEC.Match m1 = new HTEC.Match() { Id = 101, GroupName = GroupName.A, HomeTeam = "Inter", AwayTeam = "Milan", Score = "3:0", Matchday = Matchday.First };

            IEnumerable<HTEC.Match> matches1 = new List<HTEC.Match>(4)
            {
                m1,
                new HTEC.Match() { Id = 102, GroupName = GroupName.A, HomeTeam = "Juventus", AwayTeam = "Napoli", Score = "3:1", Matchday = Matchday.First },
                new HTEC.Match() { Id = 103, GroupName = GroupName.A, HomeTeam = "Napoli", AwayTeam = "Milan", Score = "2:0", Matchday = Matchday.Second },
                new HTEC.Match() { Id = 104, GroupName = GroupName.A, HomeTeam = "Inter", AwayTeam = "Juventus", Score = "1:0", Matchday = Matchday.Second }
            };

            Group group1 = new Group(GroupName.A, matches1);

            IEnumerable<HTEC.Match> matches2 = new List<HTEC.Match>(4)
            {
                new HTEC.Match() {GroupName = GroupName.C, HomeTeam = "Parma", AwayTeam = "Fiorentina", Score = "0:2", Matchday = Matchday.First },
                new HTEC.Match() {GroupName = GroupName.C, HomeTeam = "Lazio", AwayTeam = "Roma", Score = "1:1", Matchday = Matchday.First },
                new HTEC.Match() {GroupName = GroupName.C, HomeTeam = "Parma", AwayTeam = "Roma", Score = "1:2", Matchday = Matchday.Second },
                new HTEC.Match() {GroupName = GroupName.C, HomeTeam = "Fiorentina", AwayTeam = "Lazio", Score = "1:0", Matchday = Matchday.Second }
            };

            Group group2 = new Group(GroupName.C, matches2);
            
            repositoryMock.Setup(x => x.GetGroupResults(validGroupsList)).Returns(new List<Group>(2) { group1, group2 });
            repositoryMock.Setup(x => x.GetMatch(validMatchParam)).Returns(m1);
            repositoryMock.Setup(x => x.DeleteMatch(validMatchParam)).Returns(m1);
            repositoryMock.Setup(x => x.EditMatches(invalidUpdates)).Throws<ArgumentException>();
            repositoryMock.Setup(x => x.AddMatches(invalidMatches)).Throws<ArgumentException>();
            repositoryMock.Setup(x => x.AddMatches(validMatches)).Returns(new List<Group>(2) { group1, group2 });
            repositoryMock.Setup(x => x.GetMatches(invalidFilter)).Throws<ArgumentException>();
            repositoryMock.Setup(x => x.GetMatches(validFilter)).Returns(matches1);
        }

        #endregion Constructors and Initializers

        #region Tests

        [Fact]
        public void GET_GetGroups_ReturnsBadRequestIfInvalidParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Get(invalidGroupsParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void GET_GetGroups_ReturnsOkIfValidParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Get(validGroupsParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            OkObjectResult res = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Group> groups = Assert.IsType<List<Group>>(res.Value);
            Assert.True(groups.Count == 2);
        }
        
        [Fact]
        public void GET_GetMatch_ReturnsNotFoundIfInvalidParametersSent()
        {
            ActionResult<HTEC.Match> actionResult = controller.Get(invalidMatchParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<HTEC.Match>>(actionResult);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public void GET_GetMatch_ReturnsOkIfValidMatchParametersSent()
        {
            ActionResult<HTEC.Match> actionResult = controller.Get(validMatchParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<HTEC.Match>>(actionResult);
            OkObjectResult res = Assert.IsType<OkObjectResult>(actionResult.Result);
            HTEC.Match match = Assert.IsType<HTEC.Match>(res.Value);
            Assert.True(match.Id == validMatchParam);
        }
        
        [Fact]
        public void DELETE_DeleteMatch_ReturnsNotFoundIfInvalidParametersSent()
        {
            ActionResult<HTEC.Match> actionResult = controller.Delete(invalidMatchParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<HTEC.Match>>(actionResult);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public void DELETE_DeleteMatch_ReturnsOkIfValidMatchParametersSent()
        {
            ActionResult<HTEC.Match> actionResult = controller.Delete(validMatchParam);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<HTEC.Match>>(actionResult);
            OkObjectResult res = Assert.IsType<OkObjectResult>(actionResult.Result);
            HTEC.Match match = Assert.IsType<HTEC.Match>(res.Value);
            Assert.True(match.Id == validMatchParam);
        }

        [Fact]
        public void PUT_EditMatch_ReturnsBadRequestIfInvalidParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Put(invalidUpdates);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void PUT_EditMatch_ReturnsOkIfValidMatchParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Put(validUpdates);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public void POST_AddMatches_ReturnsBadRequestIfInvalidParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Post(invalidMatches);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void POST_AddMatches_ReturnsOkIfValidParametersSent()
        {
            ActionResult<IEnumerable<Group>> actionResult = controller.Post(validMatches);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<Group>>>(actionResult);
            OkObjectResult res = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<Group> groups = Assert.IsType<List<Group>>(res.Value);
            Assert.True(groups.Count == 2);
        }

        [Fact]
        public void GET_GetMatches_ReturnsBadRequestIfInvalidParametersSent()
        {
            ActionResult<IEnumerable<HTEC.Match>> actionResult = controller.GetFilteredMatches(invalidFilter);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<HTEC.Match>>>(actionResult);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void GET_GetMatches_ReturnsOkIfValidParametersSent()
        {
            ActionResult<IEnumerable<HTEC.Match>> actionResult = controller.GetFilteredMatches(validFilter);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<IEnumerable<HTEC.Match>>>(actionResult);
            OkObjectResult res = Assert.IsType<OkObjectResult>(actionResult.Result);
            List<HTEC.Match> matches = Assert.IsType<List<HTEC.Match>>(res.Value);
            Assert.True(matches.Count == 4);
        }

        #endregion Tests
    }
}
