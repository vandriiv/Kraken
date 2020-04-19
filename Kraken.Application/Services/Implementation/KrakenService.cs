using Kraken.Application.Exceptions;
using Kraken.Application.Models;
using Kraken.Application.Models.Mappers;
using Kraken.Application.Services.Interfaces;
using Kraken.Calculation.Exceptions;
using Kraken.Calculation.Field.Interfaces;
using Kraken.Calculation.Interfaces;
using Kraken.Calculation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Application.Services.Implementation
{
    public class KrakenService : IKrakenService
    {
        private readonly IKrakenNormalModesProgram _krakenNormalModeProgram;
        private readonly IFieldProgram _fieldModel;
        private readonly AcousticProblemDataMapper _acousticProblemDataMapper;
        private readonly KrakenComputingResultMapper _krakenComputingResultMapper;

        public KrakenService(IKrakenNormalModesProgram krakenNormalModeProgram, IFieldProgram fieldModel,
                             AcousticProblemDataMapper acousticProblemDataMapper,
                             KrakenComputingResultMapper krakenComputingResultMapper)
        {
            _krakenNormalModeProgram = krakenNormalModeProgram;
            _fieldModel = fieldModel;
            _acousticProblemDataMapper = acousticProblemDataMapper;
            _krakenComputingResultMapper = krakenComputingResultMapper;
        }

        public KrakenComputingResult ComputeModes(AcousticProblemData acousticProblemData)
        {
            KrakenComputingResult result = null;

            CalculatedModesInfo modesInfo;
            KrakenResult krakenResult;
            try
            {
                var profile = _acousticProblemDataMapper.MapKrakenInputProfile(acousticProblemData);
                (krakenResult,modesInfo) = _krakenNormalModeProgram.CalculateNormalModes(profile);
            }
            catch(KrakenException ex)
            {
                throw new KrakenComputingException(ex.Message);
            } 

            if (acousticProblemData.CalculateTransmissionLoss)
            {
                var fieldInput = _acousticProblemDataMapper.MapFieldInputData(acousticProblemData, modesInfo);
                var acousticFieldSnapshots = _fieldModel.CalculateFieldPressure(fieldInput);

                result = _krakenComputingResultMapper.MapFromKrakenAndFieldResult(krakenResult, acousticFieldSnapshots);

                result.TransmissionLoss.AddRange(CalculateTransmissionLossUsingAcousticSnapshots(acousticFieldSnapshots.Snapshots));
            }
            else
            {
                result = _krakenComputingResultMapper.MapFromKrakenResult(krakenResult);
            }            

            return result;
        }

        private IEnumerable<List<List<double>>> CalculateTransmissionLossUsingAcousticSnapshots(List<List<List<Complex>>> snapshots)
        {
            return snapshots.GetRange(1, snapshots.Count - 1)
                          .Select(x => x.GetRange(1, x.Count - 1)
                          .Select(y => y.GetRange(1, y.Count - 1)
                          .Select(z => z.Real == 0 ? 1E-6 : z.Real)
                          .Select(z => -20 * Math.Log10(Math.Abs(z))).ToList()).ToList());
        }
    }
}
