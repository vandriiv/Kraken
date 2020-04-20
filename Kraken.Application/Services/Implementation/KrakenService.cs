using Kraken.Common.Mappers;
using Kraken.Application.Exceptions;
using Kraken.Application.Models;
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

        private readonly IMapper<AcousticProblemData, KrakenInputProfile> _krakenInputProfileMapper;
        private readonly IMapper<FieldComputingRequiredData, FieldInputData> _fieldInputDataMapper;
        private readonly IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult> _krakenComputingResultMapper;

        public KrakenService(IKrakenNormalModesProgram krakenNormalModeProgram,
            IFieldProgram fieldModel,
            IMapper<AcousticProblemData, KrakenInputProfile> krakenInputProfileMapper,
            IMapper<FieldComputingRequiredData, FieldInputData> fieldInputDataMapper,
            IMapper<KrakenResultAndAcousticFieldSnapshots, KrakenComputingResult> krakenComputingResultMapper)
        {
            _krakenNormalModeProgram = krakenNormalModeProgram;
            _fieldModel = fieldModel;
            _krakenInputProfileMapper = krakenInputProfileMapper;
            _fieldInputDataMapper = fieldInputDataMapper;
            _krakenComputingResultMapper = krakenComputingResultMapper;
        }

        public KrakenComputingResult ComputeModes(AcousticProblemData acousticProblemData)
        {         
            CalculatedModesInfo modesInfo;
            KrakenResultAndAcousticFieldSnapshots krakenResultAndAcousticFieldSnapshots = new KrakenResultAndAcousticFieldSnapshots();
            
            try
            {
                var profile = _krakenInputProfileMapper.Map(acousticProblemData);
                (krakenResultAndAcousticFieldSnapshots.KrakenResult, modesInfo) = 
                    _krakenNormalModeProgram.CalculateNormalModes(profile);
            }
            catch(KrakenException ex)
            {
                throw new KrakenComputingException(ex.Message);
            } 

            if (acousticProblemData.CalculateTransmissionLoss)
            {
                var fieldComputingRequiredData = new FieldComputingRequiredData
                {
                    AcousticProblemData = acousticProblemData,
                    ModesInfo = modesInfo
                };

                var fieldInput = _fieldInputDataMapper.Map(fieldComputingRequiredData);

                krakenResultAndAcousticFieldSnapshots.AcousticFieldSnapshots = _fieldModel.CalculateFieldPressure(fieldInput);

                krakenResultAndAcousticFieldSnapshots.TransmissionLoss.AddRange(
                    CalculateTransmissionLossUsingAcousticSnapshots(krakenResultAndAcousticFieldSnapshots.
                    AcousticFieldSnapshots.Snapshots)
                );
            }

            var result = _krakenComputingResultMapper.Map(krakenResultAndAcousticFieldSnapshots);          

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
