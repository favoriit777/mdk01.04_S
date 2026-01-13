using System;
using System.Collections.Generic;

// Абстрактный базовый класс
public abstract class WeatherPhenomenon
{
    public string Name { get; protected set; }
    public string Severity { get; set; } // например, слабое, умеренное, сильное

    public WeatherPhenomenon(string name)
    {
        Name = name;
    }

    // Абстрактные методы
    public abstract void MeasureIntensity();
    public abstract void PredictDuration();
    public abstract void CalculateImpact();

    // Виртуальный метод
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Явление: {Name}");
        Console.WriteLine($"Сложность: {Severity}");
    }
}

// Производный класс - дождь
public class Rain : WeatherPhenomenon
{
    public double PrecipitationMM { get; private set; } // мм/час
    public int DurationMinutes { get; private set; }

    public Rain(string severity, double precipitationMM, int durationMinutes) : base("Дождь")
    {
        Severity = severity;
        PrecipitationMM = precipitationMM;
        DurationMinutes = durationMinutes;
    }

    public override void MeasureIntensity()
    {
        Console.WriteLine($"Измерение интенсивности дождя: {PrecipitationMM} мм/ч");
    }

    public override void PredictDuration()
    {
        Console.WriteLine($"Предполагаемая продолжительность дождя: {DurationMinutes} минут");
    }

    public override void CalculateImpact()
    {
        Console.WriteLine("Расчет воздействия дождя на окружающую среду...");
        // Можно добавить свою логику
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Интенсивность: {PrecipitationMM} мм/ч");
        Console.WriteLine($"Длительность: {DurationMinutes} мин");
        Console.WriteLine();
    }
}

// Производный класс - снег
public class Snow : WeatherPhenomenon
{
    public double SnowDepthCM { get; private set; } // см
    public string Temperature { get; private set; }

    public Snow(string severity, double snowDepthCM, string temperature) : base("Снег")
    {
        Severity = severity;
        SnowDepthCM = snowDepthCM;
        Temperature = temperature;
    }

    public override void MeasureIntensity()
    {
        Console.WriteLine($"Глубина снега: {SnowDepthCM} см");
    }

    public override void PredictDuration()
    {
        Console.WriteLine("Продолжительность снегопада зависит от погодных условий");
    }

    public override void CalculateImpact()
    {
        Console.WriteLine("Расчет воздействия снега на транспорт и инфраструктуру...");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Глубина снега: {SnowDepthCM} см");
        Console.WriteLine($"Температура: {Temperature}");
        Console.WriteLine();
    }
}

// Производный класс - ветер
public class Wind : WeatherPhenomenon
{
    public int WindSpeedKmh { get; private set; }

    public Wind(string severity, int windSpeedKmh) : base("Ветер")
    {
        Severity = severity;
        WindSpeedKmh = windSpeedKmh;
    }

    public override void MeasureIntensity()
    {
        Console.WriteLine($"Скорость ветра: {WindSpeedKmh} км/ч");
    }

    public override void PredictDuration()
    {
        Console.WriteLine("Длительность ветра зависит от фронтов");
    }

    public override void CalculateImpact()
    {
        Console.WriteLine("Расчет воздействия ветра на здания, деревья...");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Скорость ветра: {WindSpeedKmh} км/ч");
        Console.WriteLine();
    }
}

// Производный класс - туман
public class Fog : WeatherPhenomenon
{
    public int VisibilityMeters { get; private set; }

    public Fog(string severity, int visibilityMeters) : base("Туман")
    {
        Severity = severity;
        VisibilityMeters = visibilityMeters;
    }

    public override void MeasureIntensity()
    {
        Console.WriteLine($"Визуальная видимость: {VisibilityMeters} метров");
    }

    public override void PredictDuration()
    {
        Console.WriteLine("Продолжительность тумана зависит от погодных условий");
    }

    public override void CalculateImpact()
    {
        Console.WriteLine("Могут возникнуть проблемы с видимостью для транспорта...");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Видимость: {VisibilityMeters} метров");
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        List<WeatherPhenomenon> phenomena = new List<WeatherPhenomenon>
        {
            new Rain("Умеренное", 10.5, 60),
            new Snow("Сильное", 20, "-5°C"),
            new Wind("Слабый", 15),
            new Fog("Низкая", 200)
        };

        Console.WriteLine("Информация о метео-явлениях:\n");
        foreach (var phenomenon in phenomena)
        {
            phenomenon.DisplayInfo(); // полиморфизм
            phenomenon.MeasureIntensity();
            phenomenon.PredictDuration();
            phenomenon.CalculateImpact();
            Console.WriteLine("-----------------------------------");
        }
    }
}