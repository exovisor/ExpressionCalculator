using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ExpressionCalculator.Controllers;
using ExpressionCalculator.Exceptions;
using ExpressionCalculator.Models;

namespace ExpressionCalculator.Extensions
{
    internal static class StringExtensions
    {
        public static IEnumerable<ExpressionToken> SeparateTokens(this string input)
        {
            var exp = input.Minify().HandleUnaryMinus();
            var builder = new StringBuilder(exp.Length);

            var c = 0;
            while (c < exp.Length)
            {
                builder.Append(exp[c]);
                var type = ExpressionTokenType.Unknown;
                if (IsSpecialToken(exp[c], out var value))
                {
                    builder.Append(value);
                } else if (char.IsDigit(exp[c]))
                {
                    type = ExpressionTokenType.Number;
                    for (int i = c + 1; i < exp.Length; i++)
                    {
                        if (char.IsDigit(exp[i]) || exp[i] == ',')
                        {
                            builder.Append(exp[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                } else if (char.IsLetter(exp[c]))
                {
                    type = ExpressionTokenType.Functional;
                    var hasClosingBracket = false;
                    for (int i = c + 1; i < exp.Length; i++)
                    {
                        if (exp[i] == '(')
                        {
                            for (int j = i + 1; j < exp.Length; j++)
                            {
                                if (exp[j] == ')')
                                {
                                    hasClosingBracket = true;
                                    break;
                                }
                            }
                            break;
                        }

                        if (char.IsLetter(exp[i]) || char.IsDigit(exp[i]))
                        {
                            builder.Append(exp[i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!hasClosingBracket)
                    {
                        throw new ExpressionFormatException("Закрыты не все скобки");
                    }

                    if (!NotationController.s_standardFunctions.Contains(builder.ToString()))
                    {
                        throw new ExpressionFormatException($"Неизвестная функция `{builder.ToString()}`");
                    }
                } else if (NotationController.s_standardOperators.Contains(exp[c].ToString()))
                {
                    type = ExpressionTokenType.Operator;
                }
                else
                {
                    throw new ExpressionFormatException();
                }

                yield return new ExpressionToken(type, builder.ToString());
                
                c += builder.Length;
                builder.Clear();
            }
        }

        private static string Minify(this string input)
        {
            return input.ToLower().Replace(" ", "");
        }

        private static string HandleUnaryMinus(this string input)
        {
            var builder = new StringBuilder(input.Length);
            var lastType = ExpressionTokenType.Unknown;
            
            var c = 0;
            while (c < input.Length)
            {
                if (input[c] == '-' && lastType != ExpressionTokenType.Number)
                {
                    builder.Append("unaryminus(");
                    var offset = 1;
                    for (int i = c + offset; i < input.Length; i++)
                    {
                        if (char.IsDigit(input[i]) || input[i] == ',')
                        {
                            builder.Append(input[i]);
                            offset++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    builder.Append(')');

                    lastType = ExpressionTokenType.Number;
                    c += offset;
                }
                else
                {
                    if (char.IsDigit(input[c]) || input[c] == ')')
                    {
                        lastType = ExpressionTokenType.Number;
                    } else
                    {
                        lastType = ExpressionTokenType.Unknown;
                    }
                    builder.Append(input[c]);
                    c++;
                }
            }

            return builder.ToString();
        }

        private static bool IsSpecialToken(char ch, out double value)
        {
            var hasAssociation = true;
            switch (ch)
            {
                case 'π':
                    value = Math.PI;
                    break;
                default:
                    value = 0;
                    hasAssociation = false;
                    break;
            }

            return hasAssociation;
        }
    }
}
