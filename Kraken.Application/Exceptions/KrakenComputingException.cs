using System;
using System.Collections.Generic;
using System.Text;

namespace Kraken.Application.Exceptions
{
    public class KrakenComputingException : Exception
    {
        public KrakenComputingException(string message):base(message)
        {

        }
    }
}
