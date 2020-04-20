using Kraken.WebUI.Models;
using Kraken.WebUI.Models.Common;
using Moq;
using System.Collections.Generic;

namespace Kraken.WebUI.Tests.Unit.Mocks
{
    public class ModelValidatorMocks
    {
        public Mock<IModelValidator<KrakenInputModel>> ValidateReturnsNotEmptyList(IEnumerable<string> validationError)
        {
            var mock = new Mock<IModelValidator<KrakenInputModel>>();

            mock.Setup(x => x.Validate(It.IsAny<KrakenInputModel>())).Returns(new List<string>(validationError));

            return mock;
        }

        public Mock<IModelValidator<KrakenInputModel>> ValidateReturnsEmptyList()
        {
            var mock = new Mock<IModelValidator<KrakenInputModel>>();

            mock.Setup(x => x.Validate(It.IsAny<KrakenInputModel>())).Returns(new List<string>());

            return mock;
        }
    }
}
