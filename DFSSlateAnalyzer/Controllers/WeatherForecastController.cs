//using DFSSlateAnalyzerCore.Models;
//using DFSSlateAnalyzerCore.Repositories.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace DFSSlateAnalyzer.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class WeatherForecastController : ControllerBase
//    {
//        private static readonly string[] Summaries = new[]
//        {
//        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//    };

//        private readonly ILogger<WeatherForecastController> _logger;
//        private readonly ISlateRepository _slateRepository;

//        public WeatherForecastController(ILogger<WeatherForecastController> logger, ISlateRepository slateRepository)
//        {
//            _logger = logger;
//            _slateRepository = slateRepository;
//        }

//        [HttpGet(Name = "GetWeatherForecast")]
//        public IEnumerable<WeatherForecast> Get()
//        {
//            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//            {
//                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                TemperatureC = Random.Shared.Next(-20, 55),
//                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
//            })
//            .ToArray();
//        }

//        [HttpGet(Name = "GetContest")]
//        public async Task<ContestModel> GetContest(int SlateID)
//        {
//            // var contest = new Contest();

//            var contest = await _slateRepository.GetContest(SlateID);

//            return contest;
//        }
//    }
//}