using Kraken.Application.Exceptions;
using Kraken.Application.Services.Interfaces;
using Kraken.WebUI.Models;
using Kraken.WebUI.Models.Common;
using Kraken.WebUI.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Kraken.WebUI.Controllers
{
    public class KrakenController:Controller
    {
        private readonly IKrakenService _krakenService;
        private readonly KrakenInputModelMapper _krakenInputModelMapper;
        private readonly KrakenResultModelMapper _krakenResultModelMapper;
        private readonly IModelValidator<KrakenInputModel> _krakenInputModelValidator;

        public KrakenController(IKrakenService krakenService,
                                KrakenInputModelMapper krakenInputModelMapper,
                                KrakenResultModelMapper krakenResultModelMapper,
                                IModelValidator<KrakenInputModel> krakenInputModelValidator)
        {
            _krakenService = krakenService;
            _krakenInputModelMapper = krakenInputModelMapper;
            _krakenResultModelMapper = krakenResultModelMapper;
            _krakenInputModelValidator = krakenInputModelValidator;
        }

        [HttpPost]
        public IActionResult ComputeNormalModes([FromBody] KrakenInputModel model)
        {           
            if(model == null)
            {
                return BadRequest("Input data is not valid");
            }

            var errors = _krakenInputModelValidator.Validate(model);
            if (errors.Any())
            {
                return BadRequest(new { validationErrors= errors });
            }

            var acousticProblem = _krakenInputModelMapper.MapAcousticProblemData(model);

            try
            {
                var computingResult = _krakenService.ComputeModes(acousticProblem);

                var viewModel = _krakenResultModelMapper.MapKrakenComputingResult(computingResult);

                return Json(viewModel);
            }
            catch(KrakenComputingException ex)
            {
                return StatusCode(500, new { expectedError=true,error=ex.Message });
            }
        }
    }
}
