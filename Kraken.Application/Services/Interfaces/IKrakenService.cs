using Kraken.Application.Models;

namespace Kraken.Application.Services.Interfaces
{
    public interface IKrakenService
    {
        KrakenComputingResult ComputeModes(AcousticProblemData acousticProblemData);
    }
}
