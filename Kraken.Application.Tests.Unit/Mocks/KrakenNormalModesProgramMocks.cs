using Kraken.Calculation.Exceptions;
using Kraken.Calculation.Interfaces;
using Kraken.Calculation.Models;
using Moq;

namespace Kraken.Application.Tests.Unit.Mocks
{
    class KrakenNormalModesProgramMocks
    {
        public Mock<IKrakenNormalModesProgram> CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo()
        {
            var mock = new Mock<IKrakenNormalModesProgram>();

            mock.Setup(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>())).Returns((new KrakenResult(), new CalculatedModesInfo()));

            return mock;
        }

        public Mock<IKrakenNormalModesProgram> CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo(KrakenResult krakenResult, CalculatedModesInfo calculatedModesInfo)
        {
            var mock = new Mock<IKrakenNormalModesProgram>();

            mock.Setup(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>())).Returns((krakenResult, calculatedModesInfo));

            return mock;
        }

        public Mock<IKrakenNormalModesProgram> CalculateNormalModesThrowsKrakenExceptionWithMessage(string message)
        {
            var mock = new Mock<IKrakenNormalModesProgram>();

            mock.Setup(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>())).Throws(new KrakenException(message));
           
            return mock;
        }
       
    }
}
