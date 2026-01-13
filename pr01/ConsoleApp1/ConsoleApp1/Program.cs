using System;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        Console.WriteLine("Информация о текущей платформе:");
        Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"Runtime Identifier: {RuntimeInformation.RuntimeIdentifier}");
        // Или можно использовать Environment.OSVersion, но RuntimeInformation — более современное.
        Console.WriteLine($"OS Version: {Environment.OSVersion}");
    }
}