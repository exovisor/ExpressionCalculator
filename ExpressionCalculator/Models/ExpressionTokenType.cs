using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionCalculator.Models
{
    internal enum ExpressionTokenType
    {
        Unknown = 0,
        Number,
        Operator,
        Functional,
    }
}
