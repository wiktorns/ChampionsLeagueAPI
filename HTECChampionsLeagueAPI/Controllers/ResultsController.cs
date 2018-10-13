using HTECChampionsLeagueAPI.Models;
using HTECChampionsLeagueAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HTECChampionsLeagueAPI.Controllers
{
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IMatchRepository _repository;

        public ResultsController(IMatchRepository repository)
        {
            _repository = repository;
        }

        #region HTTP Get

        [HttpGet]
        [Route("api/results")]
        public ActionResult<IEnumerable<Group>> Get([FromQuery] string groups)
        {
            try
            {
                IEnumerable<string> groupsList = groups != null ? groups.Split(',') : null;

                return Ok(_repository.GetGroupResults(groupsList));
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }


        [HttpGet]
        [Route("api/matches")]
        public ActionResult<IEnumerable<Match>> GetFilteredMatches([FromQuery] MatchFilter filter)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_repository.GetMatches(filter));
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        /*
        [HttpGet]
        [Route("api/matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetFilteredMatches([FromQuery] MatchFilter filter)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IEnumerable<Match> res = await _repository.GetMatchesAsync(filter);

                return Ok(res);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }
        */
        [HttpGet("{id}")]
        [Route("api/results/{id}")]
        public ActionResult<Match> Get(int id)
        {
            Match m = _repository.GetMatch(id);

            if (m != null)
            {
                return Ok(m);
            }
            else
            {
                return NotFound($"Match with id {id} does not exist");
            }
        }

        //[HttpGet("{id}")]
        //[Route("api/results/{id}")]
        //public async Task<ActionResult<Match>> Get(int id)
        //{
        //    Match m = await _repository.GetMatchAsync(id);

        //    if (m != null)
        //    {
        //        return Ok(m);
        //    }
        //    else
        //    {
        //        return NotFound($"Match with id {id} does not exist");
        //    }
        //}
        #endregion HTTPS Get

        #region HTTP Post

        [HttpPost]
        [Route("api/results")]
        public ActionResult<IEnumerable<Group>> Post([FromBody] IEnumerable<Match> matches)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_repository.AddMatches(matches));
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        #endregion HTTP Post

        #region HTTPS Put

        [HttpPut]
        [Route("api/results")]
        public ActionResult<IEnumerable<Group>> Put([FromBody] IEnumerable<ScoreUpdate> scoreUpdates)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_repository.EditMatches(scoreUpdates));
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        #endregion HTTP Put

        #region HTTP Delete

        [HttpDelete("{id}")]
        [Route("api/results/delete/{id}")]
        public ActionResult<Match> Delete(int id)
        {
            Match m = _repository.DeleteMatch(id);

            if (m != null)
            {
                return Ok(m);
            }
            else
            {
                return NotFound($"Match with id {id} does not exist");
            }
        }

        #endregion HTTP Delete
    }
}
