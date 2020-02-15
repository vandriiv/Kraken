using Kraken.Application.Models;

namespace Kraken.Application.Services.Interfaces
{
    public interface IKrakenService
    {
        NormalModes ComputeModes(AcousticProblemData acousticProblemData);
    }
}
