using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DFSSlateAnalyzerAngular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContestController : ControllerBase
    {


        private readonly ILogger<ContestController> _logger;
        private readonly ISlateRepository _slateRepository;

        public ContestController(ILogger<ContestController> logger, ISlateRepository slateRepository)
        {
            _logger = logger;
            _slateRepository = slateRepository;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet(Name = "Contest")]
        public async Task<Contest> Contest(int SlateID = 0)
        {
            // var contest = new Contest();

            var contest = await _slateRepository.LoadContest(SlateID);

            return contest;
        }


    }
}