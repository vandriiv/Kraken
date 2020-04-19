using Kraken.Application.Exceptions;
using Kraken.Application.Models.Mappers;
using Kraken.Application.Services.Implementation;
using Kraken.Application.Tests.Unit.Mocks;
using Kraken.Calculation.Models;
using Moq;
using Xunit;

namespace Kraken.Application.Tests.Unit.Services
{
    public class KrakenServiceTests
    {
        private readonly TestDataHelper _testDataHelper;
        private readonly FieldProgramMocks _fieldProgramMocks;
        private readonly KrakenNormalModesProgramMocks _krakenNormalModesProgramMocks;

        public KrakenServiceTests()
        {
            _testDataHelper = new TestDataHelper();
            _fieldProgramMocks = new FieldProgramMocks();
            _krakenNormalModesProgramMocks = new KrakenNormalModesProgramMocks();
        }

        [Fact]
        public void ComputeModesShouldThrowsKrakenComputingExceptionWhenKrakenExceptionOccurs()
        {
            const string exceptionMessage = "test message";

            var krakenNormalProgramMock = _krakenNormalModesProgramMocks.CalculateNormalModesThrowsKrakenExceptionWithMessage(exceptionMessage);
            var fieldProgramMock = _fieldProgramMocks.CalculateFieldPressureReturnsAcousticFieldSnapshots();

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object, new AcousticProblemDataMapper(),
                                        new KrakenComputingResultMapper());

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKraken();

            var message = Assert.Throws<KrakenComputingException>(() => sut.ComputeModes(acousticProblemData)).Message;
            Assert.Equal(exceptionMessage, message);
        }

        [Fact]
        public void ComputeModesMustNotCalculateFieldPressure()
        {
            var krakenNormalProgramMock = _krakenNormalModesProgramMocks
                                           .CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo
                                           (_testDataHelper.GetKrakenResult(),
                                            null);

            var fieldProgramMock = _fieldProgramMocks.CalculateFieldPressureReturnsAcousticFieldSnapshots();

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object, new AcousticProblemDataMapper(),
                                        new KrakenComputingResultMapper());

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKraken();

            var result = sut.ComputeModes(acousticProblemData);

            krakenNormalProgramMock.Verify(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>()),Times.Once());
            fieldProgramMock.Verify(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>()),Times.Never());
        }

        [Fact]
        public void ComputeModesMustCalculateFieldPressure()
        {
            var krakenNormalProgramMock = _krakenNormalModesProgramMocks
                                           .CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo(
                                           _testDataHelper.GetKrakenResult(),
                                           _testDataHelper.GetCalculatedModesInfo());

            var fieldProgramMock = _fieldProgramMocks
                                    .CalculateFieldPressureReturnsAcousticFieldSnapshots(
                                    _testDataHelper.GetAcousticFieldSnapshots());

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object, new AcousticProblemDataMapper(),
                                        new KrakenComputingResultMapper());

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKrakenAndField();

            var result = sut.ComputeModes(acousticProblemData);

            krakenNormalProgramMock.Verify(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>()), Times.Once());
            fieldProgramMock.Verify(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>()), Times.Once());
        }
    }
}
