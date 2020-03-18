using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kraken.WebUI.Models
{
    public class TLAtSourceDepth
    {
        public double SourceDepth { get; set; }
        public List<TLAtReceiverDepth> TLAtReceiverDepths { get; set; }
    }
}
