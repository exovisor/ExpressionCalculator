namespace ExpressionCalculator.Models
{
    internal class ExpressionToken
    {
        public ExpressionTokenType Type { get; init; }
        public string Value { get; init; }

        public ExpressionToken(ExpressionTokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
