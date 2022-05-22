using ExpressionCalculator.Exceptions;
using ExpressionCalculator.Extensions;
using ExpressionCalculator.Models;

namespace ExpressionCalculator.Controllers
{
    internal static class NotationController
    {
        public static IEnumerable<string> s_standardOperators = new[]
        {
            "(",
            ")",
            "+",
            "-",
            "*",
            "/",
            "%",
            "^",
        };

        public static IEnumerable<string> s_standardFunctions = new[]
        {
            "sqrt",
            "exp",
            "sin",
            "cos",
            "tan",
            "abs",
            // Internal pointers
            "unaryminus"
        };

        public static List<ExpressionToken> ConvertToRpn(string input)
        {
            var tokens = input.SeparateTokens();
            var result = new List<ExpressionToken>();
            var stack = new Stack<ExpressionToken>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case ExpressionTokenType.Number:
                        result.Add(token);
                        break;
                    case ExpressionTokenType.Operator when token.Value == "(":
                        stack.Push(token);
                        break;
                    case ExpressionTokenType.Operator when token.Value == ")":
                    {
                        var head = stack.Pop();
                        while (head.Value != "(")
                        {
                            result.Add(head);
                            head = stack.Pop();
                        }

                        break;
                    }
                    case ExpressionTokenType.Operator:
                    case ExpressionTokenType.Functional:
                    {
                        if (stack.Count > 0 && GetPriority(token) < GetPriority(stack.Peek()))
                        {
                            result.Add(stack.Pop());
                        }

                        stack.Push(token);
                        break;
                    }
                    case ExpressionTokenType.Unknown:
                        throw new NotationException();
                }
            }

            if (stack.Count > 0)
            {
                foreach (var token in stack)
                {
                    result.Add(token);
                }
            }

            return result;
        }

        public static double Calculate(List<ExpressionToken> tokens)
        {
            var stack = new Stack<double>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case ExpressionTokenType.Number:
                    {
                        var val = double.Parse(token.Value);
                        stack.Push(val);
                        break;
                    }
                    case ExpressionTokenType.Operator:
                    {
                        if (stack.Count < 2)
                        {
                            throw new NotationException("Not enough arguments in stack to call operator");
                        }

                        var (d2, d1) = (stack.Pop(), stack.Pop());
                        var operation = GetOperation(token.Value);
                        stack.Push(operation.Invoke(d1, d2));
                        break;
                    }
                    case ExpressionTokenType.Functional:
                    {
                        // TODO: Var number of parameters; For now only supports functions with 1 parameter;
                        if (stack.Count < 1)
                        {
                            throw new NotationException("Not enough arguments in stack to call a function");
                        }

                        var d1 = stack.Pop();
                        var function = GetFunction(token.Value);
                        stack.Push(function.Invoke(d1));
                        break;
                    }
                    case ExpressionTokenType.Unknown:
                        throw new NotationException();
                }
            }

            return stack.Peek();
        }

        private static int GetPriority(ExpressionToken t) => GetPriority(t.Value);

        private static int GetPriority(string s)
        {
            if (s_standardFunctions.Contains(s))
            {
                return 9;
            }

            return s switch
            {
                "(" => 1,
                ")" => 2,
                "+" => 3,
                "-" => 4,
                "%" => 5,
                "*" => 6,
                "/" => 7,
                "^" => 8,
                _ => 0
            };
        }

        private static Func<double, double, double> GetOperation(string op)
        {
            return op switch
            {
                "+" => (d1, d2) => d1 + d2,
                "-" => (d1, d2) => d1 - d2,
                "%" => (d1, d2) => d1 % d2,
                "*" => (d1, d2) => d1 * d2,
                "/" => (d1, d2) => d1 / d2,
                "^" => Math.Pow,
                _ => throw new NotationException($"Unknown operation `{op}`")
            };
        }

        private static Func<double, double> GetFunction(string fn)
        {
            return fn switch
            {
                "sqrt" => Math.Sqrt,
                "exp" => Math.Exp,
                "sin" => Math.Sin,
                "cos" => Math.Cos,
                "tan" => Math.Tan,
                "abs" => Math.Abs,
                "unaryminus" => (d) =>- d,
                _ => throw new NotationException($"Unknown function `{fn}`")
            };
        }
    }
}
