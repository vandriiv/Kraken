using Kraken.WebUI.Controllers;
using Kraken.WebUI.Models;
using Kraken.WebUI.Tests.Unit.Helpers;
using Kraken.WebUI.Tests.Unit.Mocks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kraken.WebUI.Tests.Unit.Controllers
{
    public class KrakenControllerTests
    {
        private readonly KrakenServiceMocks _krakenServiceMocks;
        private readonly MappersMocks _mappersMocks;
        private readonly ModelValidatorMocks _modelValidatorMocks;

        public KrakenControllerTests()
        {
            _krakenServiceMocks = new KrakenServiceMocks();
            _mappersMocks = new MappersMocks();
            _modelValidatorMocks = new ModelValidatorMocks();
        }

        [Fact]
        public void ComputeNormalModesReturnsBadRequestWithMessageWhenModelIsNull()
        {
            const string message = "Input data is not valid";

            var krakenServiceMock = _krakenServiceMocks.ComputeModesReturnsKrakenComputingResult();
            var krakenInputModelMapperMock = _mappersMocks.KrakenInputModelMapper();
            var krakenResultModelMapperMock = _mappersMocks.KrakenResultModelMapper();
            var krakenInputModelValidator = _modelValidatorMocks.ValidateReturnsEmptyList();

            var krakenController = new KrakenController(krakenServiceMock.Object, krakenInputModelMapperMock.Object,
                                                        krakenResultModelMapperMock.Object, krakenInputModelValidator.Object);

            var badRequest = krakenController.ComputeNormalModes(null) as BadRequestObjectResult;

            Assert.NotNull(badRequest);

            Assert.Equal(message, badRequest.Value);
        }

        [Fact]
        public void ComputeNormalModesReturnsBadRequestWithValidationErrorsWhenModelIsNotValid()
        {
            var validationErrors = new List<string> { "error 1", "error 2" };

            var krakenServiceMock = _krakenServiceMocks.ComputeModesReturnsKrakenComputingResult();
            var krakenInputModelMapperMock = _mappersMocks.KrakenInputModelMapper();
            var krakenResultModelMapperMock = _mappersMocks.KrakenResultModelMapper();
            var krakenInputModelValidator = _modelValidatorMocks.ValidateReturnsNotEmptyList(validationErrors);

            var krakenController = new KrakenController(krakenServiceMock.Object, krakenInputModelMapperMock.Object,
                                                        krakenResultModelMapperMock.Object, krakenInputModelValidator.Object);
            var model = new KrakenInputModel();

            var badRequest = krakenController.ComputeNormalModes(model) as BadRequestObjectResult;

            dynamic obj = new DynamicObjectResultValue(badRequest.Value);
            var returnedValidationErrors = obj.validationErrors as List<string>;

            Assert.NotNull(badRequest);

            Assert.NotNull(returnedValidationErrors);

            Assert.Equal(returnedValidationErrors.Count, validationErrors.Count);
            Assert.Contains(validationErrors[0], returnedValidationErrors);
            Assert.Contains(validationErrors[1], returnedValidationErrors);
        }

        [Fact]
        public void ComputeNormalModesReturnsJsonResultWithKrakenResultModel()
        {
            var krakenServiceMock = _krakenServiceMocks.ComputeModesReturnsKrakenComputingResult();
            var krakenInputModelMapperMock = _mappersMocks.KrakenInputModelMapper();
            var krakenResultModelMapperMock = _mappersMocks.KrakenResultModelMapper();
            var krakenInputModelValidator = _modelValidatorMocks.ValidateReturnsEmptyList();

            var krakenController = new KrakenController(krakenServiceMock.Object, krakenInputModelMapperMock.Object,
                                                        krakenResultModelMapperMock.Object, krakenInputModelValidator.Object);

            var model = new KrakenInputModel();

            var jsonResult = krakenController.ComputeNormalModes(model) as JsonResult;

            Assert.NotNull(jsonResult);          

            Assert.IsAssignableFrom<KrakenResultModel>(jsonResult.Value);
        }

        [Fact]
        public void ComputeNormalModesReturns500StatusCodeWhenKrakenComputingExceptionOccurs()
        {
            const string message = "error";
            const int statusCode500 = 500;

            var krakenServiceMock = _krakenServiceMocks.ComputeModesThrowsKrakenComputingException(message);
            var krakenInputModelMapperMock = _mappersMocks.KrakenInputModelMapper();
            var krakenResultModelMapperMock = _mappersMocks.KrakenResultModelMapper();
            var krakenInputModelValidator = _modelValidatorMocks.ValidateReturnsEmptyList();

            var krakenController = new KrakenController(krakenServiceMock.Object, krakenInputModelMapperMock.Object,
                                                        krakenResultModelMapperMock.Object, krakenInputModelValidator.Object);

            var model = new KrakenInputModel();

            var objectResult = krakenController.ComputeNormalModes(model) as ObjectResult;

            Assert.NotNull(objectResult);

            Assert.Equal(statusCode500, objectResult.StatusCode);

            dynamic obj = new DynamicObjectResultValue(objectResult.Value);

            Assert.True(obj.expectedError);
            Assert.Equal(message, obj.error);
        }
    }
}
