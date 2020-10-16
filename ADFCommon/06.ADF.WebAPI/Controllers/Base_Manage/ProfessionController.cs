using Microsoft.AspNetCore.Mvc;
using ADF.IBusiness;

namespace ADF.WebAPI.Controllers
{
    public class ProfessionController : BaseController
    {
        IProfessionBussiness _professionBus { get; }

        public ProfessionController(IProfessionBussiness professionBus)
        {
            _professionBus = professionBus;
        }

        [HttpGet]
        public IActionResult ExportData()
        {
            // _professionBus.GetXMLData();
            return Ok("");
        }

        [HttpPost]
        public IActionResult ImportData()
        {
            // _professionBus.UpdateProfession(null);
            return Ok("");
        }
    }
}