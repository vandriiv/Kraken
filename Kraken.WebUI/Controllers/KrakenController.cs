using Kraken.Application.Services.Interfaces;
using Kraken.WebUI.Models;
using Kraken.WebUI.Models.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Kraken.WebUI.Controllers
{
    public class KrakenController:Controller
    {
        private readonly IKrakenService _krakenService;
        private readonly KrakenInputModelMapper _krakenInputModelMapper;
        private readonly KrakenResultModelMapper _krakenResultModelMapper;

        public KrakenController(IKrakenService krakenService,
                                KrakenInputModelMapper krakenInputModelMapper,
                                KrakenResultModelMapper krakenResultModelMapper)
        {
            _krakenService = krakenService;
            _krakenInputModelMapper = krakenInputModelMapper;
            _krakenResultModelMapper = krakenResultModelMapper;
        }

        [HttpPost]
        public IActionResult ComputeNormalModes([FromBody] KrakenInputModel model)
        {
            var acousticProblem = _krakenInputModelMapper.MapAcousticProblemData(model);

            var result = _krakenResultModelMapper.MapNormalModes(_krakenService.ComputeModes(acousticProblem));

            return Json(result);
        }
    }
}
