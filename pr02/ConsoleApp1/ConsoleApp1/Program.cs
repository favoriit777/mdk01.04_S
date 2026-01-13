using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите текст для преобразования:");
        string inputText = Console.ReadLine();

        Console.WriteLine("Введите тип преобразования (int, double, bool):");
        string typeInput = Console.ReadLine().Trim().ToLower();

        try
        {
            switch (typeInput)
            {
                case "int":
                    {
                        int result = Convert.ToInt32(inputText);
                        Console.WriteLine($"Преобразованное значение: {result} (тип: {result.GetType()})");
                        break;
                    }
                case "double":
                    {
                        double result = Convert.ToDouble(inputText);
                        Console.WriteLine($"Преобразованное значение: {result} (тип: {result.GetType()})");
                        break;
                    }
                case "bool":
                    {
                        bool result = Convert.ToBoolean(inputText);
                        Console.WriteLine($"Преобразованное значение: {result} (тип: {result.GetType()})");
                        break;
                    }
                default:
                    Console.WriteLine("Неизвестный тип. Используйте: int, double, bool.");
                    break;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка: неверный формат входных данных для выбранного типа.");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Ошибка: значение превышает допустимый диапазон для выбранного типа.");
        }
        catch (InvalidCastException)
        {
            Console.WriteLine("Ошибка: невозможное преобразование типов.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
        }
    }
}