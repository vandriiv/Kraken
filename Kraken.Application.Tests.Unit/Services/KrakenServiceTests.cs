using Kraken.Application.Exceptions;
using Kraken.Application.Models;
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
        private readonly MappersMocks _mappersMocks;

        public KrakenServiceTests()
        {
            _testDataHelper = new TestDataHelper();
            _fieldProgramMocks = new FieldProgramMocks();
            _krakenNormalModesProgramMocks = new KrakenNormalModesProgramMocks();
            _mappersMocks = new MappersMocks();
        }

        [Fact]
        public void ComputeModesShouldThrowsKrakenComputingExceptionWhenKrakenExceptionOccurs()
        {
            const string exceptionMessage = "test message";

            var krakenNormalProgramMock = _krakenNormalModesProgramMocks.CalculateNormalModesThrowsKrakenExceptionWithMessage(exceptionMessage);
            var fieldProgramMock = _fieldProgramMocks.CalculateFieldPressureReturnsAcousticFieldSnapshots();

            var krakenInputProfileMapperMock = _mappersMocks.KrakenInputProfileMapper(_testDataHelper.GetKrakenInputProfile());
            var fieldInputDataMapperMock = _mappersMocks.FieldInputDataMapper(_testDataHelper.GetFieldInputData());
            var krakenComputingResultMapperMock = _mappersMocks.KrakenComputingResultMapper();

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object,
                                       krakenInputProfileMapperMock.Object, fieldInputDataMapperMock.Object,
                                       krakenComputingResultMapperMock.Object);

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKraken();           

            var message = Assert.Throws<KrakenComputingException>(() => sut.ComputeModes(acousticProblemData)).Message;
            Assert.Equal(exceptionMessage, message);

            krakenInputProfileMapperMock.Verify(x => x.Map(It.IsAny<AcousticProblemData>()), Times.Once());
            fieldInputDataMapperMock.Verify(x => x.Map(It.IsAny<FieldComputingRequiredData>()), Times.Never());
            krakenComputingResultMapperMock.Verify(x => x.Map(It.IsAny<KrakenResultAndAcousticFieldSnapshots>()), Times.Never());
        }

        [Fact]
        public void ComputeModesMustNotCalculateFieldPressure()
        {
            var krakenNormalProgramMock = _krakenNormalModesProgramMocks
                                           .CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo();

            var fieldProgramMock = _fieldProgramMocks.CalculateFieldPressureReturnsAcousticFieldSnapshots();

            var krakenInputProfileMapperMock = _mappersMocks.KrakenInputProfileMapper(_testDataHelper.GetKrakenInputProfile());
            var fieldInputDataMapperMock = _mappersMocks.FieldInputDataMapper(_testDataHelper.GetFieldInputData());
            var krakenComputingResultMapperMock = _mappersMocks.KrakenComputingResultMapper();

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object,
                                       krakenInputProfileMapperMock.Object, fieldInputDataMapperMock.Object,
                                       krakenComputingResultMapperMock.Object);

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKraken();

            var result = sut.ComputeModes(acousticProblemData);

            krakenNormalProgramMock.Verify(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>()),Times.Once());
            fieldProgramMock.Verify(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>()),Times.Never());

            krakenInputProfileMapperMock.Verify(x => x.Map(It.IsAny<AcousticProblemData>()), Times.Once());
            fieldInputDataMapperMock.Verify(x => x.Map(It.IsAny<FieldComputingRequiredData>()), Times.Never());
            krakenComputingResultMapperMock.Verify(x => x.Map(It.IsAny<KrakenResultAndAcousticFieldSnapshots>()), Times.Once());
        }

        [Fact]
        public void ComputeModesMustCalculateFieldPressure()
        {
            var krakenNormalProgramMock = _krakenNormalModesProgramMocks
                                           .CalculateNormalModesReturnsKrakenResultAndCalculatedModesInfo();

            var fieldProgramMock = _fieldProgramMocks
                                    .CalculateFieldPressureReturnsAcousticFieldSnapshots(_testDataHelper.GetAcousticFieldSnapshots());

            var krakenInputProfileMapperMock = _mappersMocks.KrakenInputProfileMapper(_testDataHelper.GetKrakenInputProfile());
            var fieldInputDataMapperMock = _mappersMocks.FieldInputDataMapper(_testDataHelper.GetFieldInputData());
            var krakenComputingResultMapperMock = _mappersMocks.KrakenComputingResultMapper();

            var sut = new KrakenService(krakenNormalProgramMock.Object, fieldProgramMock.Object,
                                       krakenInputProfileMapperMock.Object, fieldInputDataMapperMock.Object,
                                       krakenComputingResultMapperMock.Object);

            var acousticProblemData = _testDataHelper.GetAcousticProblemDataForKrakenAndField();

            var result = sut.ComputeModes(acousticProblemData);

            krakenNormalProgramMock.Verify(x => x.CalculateNormalModes(It.IsAny<KrakenInputProfile>()), Times.Once());
            fieldProgramMock.Verify(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>()), Times.Once());

            krakenInputProfileMapperMock.Verify(x => x.Map(It.IsAny<AcousticProblemData>()), Times.Once());
            fieldInputDataMapperMock.Verify(x => x.Map(It.IsAny<FieldComputingRequiredData>()), Times.Once());
            krakenComputingResultMapperMock.Verify(x => x.Map(It.IsAny<KrakenResultAndAcousticFieldSnapshots>()), Times.Once());
        }
    }
}
