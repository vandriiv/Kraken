using Kraken.Common.Mappers;
using Kraken.Calculation.Models;
using System.Collections.Generic;

namespace Kraken.Application.Models.Mappers
{
    public class FieldInputDataMapper : IMapper<FieldComputingRequiredData, FieldInputData>
    {
        public FieldInputData Map(FieldComputingRequiredData source)
        {
            var options = source.AcousticProblemData.SourceType + source.AcousticProblemData.ModesTheory;

            var r = new List<double>(source.AcousticProblemData.R);
            r.Insert(0, 0);

            var rr = new List<double>(source.AcousticProblemData.RR);
            rr.Insert(0, 0);

            var sd = new List<double>(source.AcousticProblemData.SDField);
            sd.Insert(0, 0);

            var rd = new List<double>(source.AcousticProblemData.RDField);
            rd.Insert(0, 0);

            var fieldInputData = new FieldInputData(source.ModesInfo, options, source.AcousticProblemData.NModesForField,
                source.AcousticProblemData.NR, r, source.AcousticProblemData.NSDField, sd,
                source.AcousticProblemData.NRDField, rd, source.AcousticProblemData.NRR, rr);

            return fieldInputData;
        }
    }
}
