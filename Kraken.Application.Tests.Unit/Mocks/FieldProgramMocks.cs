using Kraken.Calculation.Field.Interfaces;
using Kraken.Calculation.Models;
using Moq;

namespace Kraken.Application.Tests.Unit.Mocks
{
    class FieldProgramMocks
    {
        public Mock<IFieldProgram> CalculateFieldPressureReturnsAcousticFieldSnapshots()
        {
            var mock = new Mock<IFieldProgram>();
            mock.Setup(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>())).Returns(new AcousticFieldSnapshots());

            return mock;
        }

        public Mock<IFieldProgram> CalculateFieldPressureReturnsAcousticFieldSnapshots(AcousticFieldSnapshots acousticFieldSnapshots)
        {
            var mock = new Mock<IFieldProgram>();
            mock.Setup(x => x.CalculateFieldPressure(It.IsAny<FieldInputData>())).Returns(acousticFieldSnapshots);

            return mock;
        }
    }
}
