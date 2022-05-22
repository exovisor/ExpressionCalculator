
using ExpressionCalculator.Controllers;
using ExpressionCalculator.Exceptions;
using ExpressionCalculator.Utils;

try
{
    var input = ConsoleUtils.GetExpression();

    var tokens = NotationController.ConvertToRpn(input);
    var result = NotationController.Calculate(tokens);

    Console.WriteLine($"Ваш результат: {result}");
}
catch (NotationException ex)
{
    Console.WriteLine("В указанном выражении содержится ошибка.");
}
catch (ExpressionFormatException ex)
{
    Console.WriteLine($"Ошибка в выражении: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine("Во время работы программы произошла ошибка.");
}
finally
{
    Console.WriteLine("Нажмите любую клавишу чтобы закрыть окно...");
    Console.ReadKey();
}
