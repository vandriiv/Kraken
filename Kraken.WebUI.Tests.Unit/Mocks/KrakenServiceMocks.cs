using Kraken.Application.Exceptions;
using Kraken.Application.Models;
using Kraken.Application.Services.Interfaces;
using Moq;

namespace Kraken.WebUI.Tests.Unit.Mocks
{
    public class KrakenServiceMocks
    {
        public Mock<IKrakenService> ComputeModesThrowsKrakenComputingException(string message)
        {
            var mock = new Mock<IKrakenService>();

            mock.Setup(x => x.ComputeModes(It.IsAny<AcousticProblemData>())).Throws(new KrakenComputingException(message));

            return mock;
        }

        public Mock<IKrakenService> ComputeModesReturnsKrakenComputingResult()
        {
            var mock = new Mock<IKrakenService>();

            mock.Setup(x => x.ComputeModes(It.IsAny<AcousticProblemData>())).Returns(new KrakenComputingResult());

            return mock;
        }
    }
}
