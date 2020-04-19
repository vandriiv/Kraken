using Kraken.Calculation.Models;

namespace Kraken.Calculation.Field.Interfaces
{
    public interface IFieldProgram
    {
        AcousticFieldSnapshots CalculateFieldPressure(FieldInputData fieldData);
    }
}
