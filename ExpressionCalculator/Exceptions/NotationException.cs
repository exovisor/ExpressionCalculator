using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCalculator.Exceptions
{
    internal class NotationException : Exception
    {
        public NotationException() : base() {}
        public NotationException(string message) : base(message) {}
    }
}
