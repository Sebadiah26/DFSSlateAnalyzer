using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using static DFSSlateAnalyzerAngular.Services.FileServerProviderService;

namespace DFSSlateAnalyzerAngular.Controllers
{
    
    public class HomeController : Controller
    {
        private IFileServerProvider _fileServerProvider;
        private readonly IFileProvider _fileProvider;

        public HomeController(IFileProvider fileProvider, IFileServerProvider fileServerProvider)
        {
            _fileProvider = fileProvider;
            _fileServerProvider = fileServerProvider;
        }

        [Route("home/index")]
        public IActionResult Index()
        {
            //var contents = _fileProvider.GetDirectoryContents("");
            var contents = _fileServerProvider.GetProvider("/files").GetDirectoryContents("");
            var file = _fileServerProvider.GetProvider("/files").GetFileInfo("/02-23-23/Export_2023_02_23.csv");
            Stream stream = file.CreateReadStream();

            return View(contents);
        }
    }
}
