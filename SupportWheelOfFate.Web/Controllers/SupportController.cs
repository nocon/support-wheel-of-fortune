using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SupportWheelOfFate.Core;

namespace SupportWheelOfFate.Web.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportRepository _supportRepository;

        public SupportController(ISupportRepository supportRepository)
        {
            _supportRepository = supportRepository;
        }
        
        [HttpGet("api/support/rota")]
        public JsonResult Rota()
        {
            // TODO: 1 week of scheduling ahead is hardcoded here. This should be moved to external config file.
            // TODO: May be worth moving into a service?
            var rota = _supportRepository.GetRota();
            rota.PlanShifts(
                DateTime.Now, 
                DateTime.Now.AddDays(7), 
                _supportRepository.GetPeople().ToList());
            _supportRepository.UpdateRota(rota);
            return new JsonResult(rota);
        }
        
        [HttpGet("api/support/people")]
        public JsonResult People()
        {
            var rota = _supportRepository.GetPeople();
            return new JsonResult(rota);
        }
    }
}