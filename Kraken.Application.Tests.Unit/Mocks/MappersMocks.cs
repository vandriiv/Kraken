using Kraken.Common.Mappers;
using Kraken.Application.Models;
using Kraken.Calculation.Models;
using Moq;

namespace Kraken.Application.Tests.Unit.Mocks
{
    class MappersMocks
    {
        public Mock<IMapper<AcousticProblemData, KrakenInputProfile>> KrakenInputProfileMapper(KrakenInputProfile krakenInputProfile)
        {
            var mock = new Mock<IMapper<AcousticProblemData, KrakenInputProfile>>();

            mock.Setup(x => x.Map(It.IsAny<AcousticProblemData>())).Returns(krakenInputProfile);

            return mock;
        }

        public Mock<IMapper<FieldComputingRequiredData, FieldInputData>> FieldInputDataMapper(FieldInputData fieldInputData)
        {
            var mock = new Mock<IMapper<FieldComputingRequiredData, FieldInputData>>();

            mock.Setup(x => x.Map(It.IsAny<FieldComputingRequiredData>())).Returns(fieldInputData);

            return mock;
        }

        public Mock<IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>> KrakenComputingResultMapper()
        {
            var mock = new Mock<IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>>();

            mock.Setup(x => x.Map(It.IsAny<KrakenResultAndAcousticFieldSnapshots>())).Returns(new KrakenComputingResult());

            return mock;
        }

        public Mock<IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>> KrakenComputingResultMapper(KrakenComputingResult krakenComputingResult)
        {
            var mock = new Mock<IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult>>();

            mock.Setup(x => x.Map(It.IsAny<KrakenResultAndAcousticFieldSnapshots>())).Returns(krakenComputingResult);

            return mock;
        }
    }
}
