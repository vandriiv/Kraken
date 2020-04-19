using Kraken.Calculation.Models;

namespace Kraken.Calculation.Interfaces
{
    public interface IKrakenNormalModesProgram
    {
        (KrakenResult, CalculatedModesInfo) CalculateNormalModes(KrakenInputProfile profile);
    }
}
