using Kraken.Common.Mappers;
using Kraken.Calculation.Models;
using System.Collections.Generic;

namespace Kraken.Application.Models.Mappers
{
    public class KrakenInputProfileMapper : IMapper<AcousticProblemData, KrakenInputProfile>
    {
        public KrakenInputProfile Map(AcousticProblemData source)
        {
            var options = source.InterpolationType + source.TopBCType + source.AttenuationUnits + source.AddedVolumeAttenuation;
            var bcBottom = source.BottomBCType;

            var mediumInfo = new List<List<double>>(source.MediumInfo);
            mediumInfo.Insert(0, new List<double>());
            foreach (var list in mediumInfo)
            {
                list.Insert(0, 0);
            }

            var ssp = new List<List<double>>(source.SSP);
            ssp.Insert(0, new List<double>());
            foreach (var list in ssp)
            {
                list.Insert(0, 0);
            }

            var sd = new List<double>(source.SD);
            sd.Insert(0, 0);

            var rd = new List<double>(source.RD);
            rd.Insert(0, 0);

            var topAHSP = new List<double> { 0, source.ZT, source.CPT, source.CST,
                                            source.RHOT,source.APT, source.AST};

            var twerskyParams = new List<double> { 0, source.BumDen, source.Eta, source.Xi };
            var bottomAHSP = new List<double> { 0, source.ZB, source.CPB, source.CSB,
                                            source.RHOB,source.APB, source.ASB};

            var krakenInputProfile = new KrakenInputProfile(source.NModes, source.Frequency, source.NMedia,
                options, bcBottom, mediumInfo, ssp, source.Sigma, source.CLow, source.CHigh,
                source.RMax, source.NSD, sd, source.NRD, rd, topAHSP, twerskyParams, bottomAHSP);

            return krakenInputProfile;
        }
    }
}
