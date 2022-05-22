namespace ExpressionCalculator.Utils
{
    internal static class ConsoleUtils
    {
        public static string GetExpression()
        {
            Console.Clear();
            Console.WriteLine("Введите выражение или путь к файлу:");
            var input = Console.ReadLine();

            if (File.Exists(input))
            {
                var content = File.ReadAllText(input);
                Console.WriteLine($"Загружено выражение: {content}");
                return content;
            }

            Console.WriteLine("Выражение введено вручную...");

            return input;
        }
    }
}
