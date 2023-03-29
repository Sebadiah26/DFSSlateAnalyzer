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


           SlateID = 141198362;

            // _slateRepository.UploadSlate(_date, SlateID);    //  Entries.csv  
            IFileInfo file;
            Stream stream;

            //var file = _fileServerProvider.GetProvider("/files").GetFileInfo("/02-05-23/Export_2023_02_05.csv");
            //Stream stream = file.CreateReadStream();

            //_slateRepository.UploadPlayers(_date, SlateID, stream);    //  Export_2023_02_13.csv manual all 



            //  var contents = _fileServerProvider.GetProvider("/files").GetDirectoryContents("");
            file = _fileServerProvider.GetProvider("/files").GetFileInfo("/02-05-23/contest-standings-" + SlateID + ".csv");
            stream = file.CreateReadStream();

            var contest = await _slateRepository.GetContest(_date, SlateID, stream);  //  _db.Contests  or "contest-standings-" + ID + ".csv"
            contests.Add(contest);


            file = _fileServerProvider.GetProvider("/files").GetFileInfo("/02-05-23/Export_2023_02_05.csv");
             stream = file.CreateReadStream();
            
            //reader = new StreamReader(stream);
            _slateRepository.UploadProjections(_date, SlateID, stream);    //  Export_2023_02_13.csv  


            
            
           



           


            // _slateRepository.SaveContestToDatabase(contest);

            _logger.LogInformation("Ended");
            _logger.LogInformation(DateTime.Now.ToString());

            return contests;
        }


    }
}