using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCalculator.Exceptions
{
    internal class ExpressionFormatException : Exception
    {   
        public ExpressionFormatException() : base() {}
        public ExpressionFormatException(string message) : base(message) {}
    }
}
