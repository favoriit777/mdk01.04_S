using System;
using System.Threading;

class Program
{
    static object monitorObject = new object();

    static int progress = 0;
    static string status = "Идет загрузка";
    static bool isCompleted = false;

    static void ProgressThread()
    {
        for (int i = 0; i <= 100; i += 10)
        {
            lock (monitorObject)
            {
                progress = i;
                if (i == 100)
                {
                    status = "Загрузка завершена";
                    isCompleted = true;
                }
            }
            Thread.Sleep(300);
        }
    }

    static void StatusThread()
    {
        while (true)
        {
            lock (monitorObject)
            {
                Console.WriteLine($"Прогресс: {progress}%, Статус: {status}");
                if (isCompleted)
                    break;
            }
            Thread.Sleep(500);
        }
    }

    static void Main()
    {
        Thread loadThread = new Thread(ProgressThread);
        Thread statusThread = new Thread(StatusThread);

        loadThread.Start();
        statusThread.Start();

        loadThread.Join();
        statusThread.Join();

        Console.WriteLine("Работа завершена.");
    }
}