using DFSSlateAnalyzerCore.Models;
using DFSSlateAnalyzerData.Data;
using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using static DFSSlateAnalyzerCore.Services.FileServerProviderService;
//using static DFSSlateAnalyzerAPI.Services.FileServerProviderService;

namespace DFSSlateAnalyzerAngular.Controllers
{
    

    [ApiController]
    [Route("[controller]")]
    public class ContestController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;
        private IFileServerProvider _fileServerProvider;

        ILogger _logger;
        private readonly ISlateRepository _slateRepository;

        public ContestController(ILoggerFactory loggerFactory, ISlateRepository slateRepository, IFileProvider fileProvider
            , IFileServerProvider fileServerProvider
            )
        {
            _logger =  loggerFactory.CreateLogger(nameof(ContestController));
            _slateRepository = slateRepository;
            _fileProvider = fileProvider;
            _fileServerProvider = fileServerProvider;
        }

      

        [HttpGet]
        public async Task<List<ContestModel>> GetContest(Int64 SlateID = 0, DateTime? date = null )
        {
             var contests = new List<ContestModel>();
             var _date = date ?? DateTime.UtcNow;
           

            _logger.LogInformation("Started");
            _logger.LogInformation(DateTime.Now.ToString());


           SlateID = 141599279;

            // _slateRepository.UploadSlate(_date, SlateID);    //  Entries.csv  

            //  _slateRepository.UploadPlayers(_date, SlateID);    //  Export_2023_02_13.csv manual all 


          //  var contents = _fileServerProvider.GetProvider("/files").GetDirectoryContents("");
            var file = _fileServerProvider.GetProvider("/files").GetFileInfo("/02-23-23/Export_2023_02_23.csv");
            Stream stream = file.CreateReadStream();
            
            //reader = new StreamReader(stream);
            _slateRepository.UploadProjections(_date, SlateID, stream);    //  Export_2023_02_13.csv  


            var contest = await _slateRepository.GetContest(_date, SlateID);  //  _db.Contests  or "contest-standings-" + ID + ".csv"



            contests.Add(contest);


            // _slateRepository.SaveContestToDatabase(contest);

            _logger.LogInformation("Ended");
            _logger.LogInformation(DateTime.Now.ToString());

            return contests;
        }


    }
}