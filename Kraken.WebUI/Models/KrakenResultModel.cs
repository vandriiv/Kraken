using System.Collections.Generic;

namespace Kraken.WebUI.Models
{
    public class KrakenResultModel
    {
        public IEnumerable<double> K { get; set; }
        public IEnumerable<double> Alpha { get; set; }
        public IEnumerable<double> PhaseSpeed { get; set; }
        public IEnumerable<double> GroupSpeed { get; set; }
        public IDictionary<double,List<double>> Modes { get; set; }
    }
}
