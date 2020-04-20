using Kraken.Application.Models;
using Kraken.Common.Mappers;
using Kraken.WebUI.Models;
using Moq;

namespace Kraken.WebUI.Tests.Unit.Mocks
{
    class MappersMocks
    {
        public Mock<IMapper<KrakenInputModel, AcousticProblemData>> KrakenInputModelMapper()
        {
            var mock = new Mock<IMapper<KrakenInputModel, AcousticProblemData>>();

            mock.Setup(x => x.Map(It.IsAny<KrakenInputModel>())).Returns(new AcousticProblemData());

            return mock;
        }

        public Mock<IMapper<KrakenComputingResult, KrakenResultModel>> KrakenResultModelMapper()
        {
            var mock = new Mock<IMapper<KrakenComputingResult, KrakenResultModel>>();

            mock.Setup(x => x.Map(It.IsAny<KrakenComputingResult>())).Returns(new KrakenResultModel());

            return mock;
        }
    }
}
