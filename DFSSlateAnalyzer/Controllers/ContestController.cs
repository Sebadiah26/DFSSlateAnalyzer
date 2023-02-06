using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerData.Data;
using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFSSlateAnalyzerAngular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContestController : ControllerBase
    {


        ILogger _logger;
        private readonly ISlateRepository _slateRepository;

        public ContestController(ILoggerFactory loggerFactory, ISlateRepository slateRepository)
        {
            _logger =  loggerFactory.CreateLogger(nameof(ContestController));
            _slateRepository = slateRepository;
        }

      

        [HttpGet]
        public async Task<List<ContestModel>> GetContest(Int64 SlateID = 0)
        {
             var contests = new List<ContestModel>();

            _logger.LogInformation("Started");
            _logger.LogInformation(DateTime.Now.ToString());


            SlateID = 141198362;
            var contest = await _slateRepository.GetContest(SlateID);
            _slateRepository.UploadProjections(SlateID);


            contests.Add(contest);


            // _slateRepository.SaveContestToDatabase(contest);

            _logger.LogInformation("Ended");
            _logger.LogInformation(DateTime.Now.ToString());

            return contests;
        }


    }
}