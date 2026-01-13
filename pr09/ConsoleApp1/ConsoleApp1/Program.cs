using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace ThreadProgressDemo
{
    /// <summary>
    /// Класс, представляющий задачу загрузки с прогрессом
    /// </summary>
    public class DownloadTask
    {
        public int Id { get; }
        public string Name { get; }
        public int Progress { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; private set; }
        public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;

        public DownloadTask(int id, string name)
        {
            Id = id;
            Name = name;
            Progress = 0;
            IsCompleted = false;
            StartTime = DateTime.Now;
        }

        public void UpdateProgress(int progress)
        {
            Progress = Math.Clamp(progress, 0, 100);
            if (Progress == 100)
            {
                IsCompleted = true;
                EndTime = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Менеджер загрузок с многопоточностью
    /// </summary>
    public class DownloadManager
    {
        private readonly object _lockObject = new object();
        private readonly ConcurrentDictionary<int, DownloadTask> _tasks = new();
        private readonly ConcurrentDictionary<int, Thread> _threads = new();
        private readonly Random _random = new Random();
        private bool _isRunning = true;

        /// <summary>
        /// Запуск нескольких загрузок параллельно
        /// </summary>
        public void StartMultipleDownloads(int count)
        {
            Console.WriteLine($"🚀 Запуск {count} параллельных загрузок...\n");

            for (int i = 1; i <= count; i++)
            {
                var task = new DownloadTask(i, $"Файл_{i}.zip");
                _tasks.TryAdd(i, task);

                // Создание и запуск потока для каждой загрузки
                var thread = new Thread(() => DownloadFile(task));
                thread.Name = $"DownloadThread-{i}";
                _threads.TryAdd(i, thread);
                thread.Start();

                Thread.Sleep(100); // Небольшая задержка между запусками потоков
            }
        }

        /// <summary>
        /// Метод, выполняемый в потоке для имитации загрузки файла
        /// </summary>
        private void DownloadFile(DownloadTask task)
        {
            try
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] Начата загрузка: {task.Name}");

                // Имитация загрузки с разной скоростью
                int speed = _random.Next(50, 300); // мс на 1% прогресса

                for (int i = 0; i <= 100; i++)
                {
                    if (!_isRunning) // Проверка на остановку
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Загрузка прервана: {task.Name}");
                        return;
                    }

                    // Обновление прогресса с синхронизацией
                    lock (_lockObject)
                    {
                        task.UpdateProgress(i);
                    }

                    // Вывод прогресса в консоль с синхронизацией
                    lock (_lockObject)
                    {
                        Console.ForegroundColor = GetProgressColor(task.Progress);
                        Console.Write($"[{Thread.CurrentThread.Name}] {task.Name}: ");
                        DrawProgressBar(task.Progress);
                        Console.ResetColor();
                    }

                    // Имитация работы - случайная задержка
                    Thread.Sleep(speed + _random.Next(-50, 50));

                    // Случайные события во время загрузки
                    HandleRandomEvents(task);
                }

                Console.WriteLine($"\n[{Thread.CurrentThread.Name}] ✅ Загрузка завершена: {task.Name} " +
                                  $"(Время: {task.Duration.TotalSeconds:F1} сек)");
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] ❌ Поток прерван: {task.Name}");
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] ⚠️ Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработка случайных событий во время загрузки
        /// </summary>
        private void HandleRandomEvents(DownloadTask task)
        {
            int eventChance = _random.Next(100);

            if (eventChance < 5) // 5% шанс на паузу
            {
                lock (_lockObject)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n[{Thread.CurrentThread.Name}] ⏸️ Пауза загрузки: {task.Name}");
                    Console.ResetColor();
                }
                Thread.Sleep(2000); // Пауза 2 секунды
            }
            else if (eventChance < 8) // 3% шанс на скачок скорости
            {
                lock (_lockObject)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[{Thread.CurrentThread.Name}] ⚡ Скачок скорости: {task.Name}");
                    Console.ResetColor();
                }
                Thread.Sleep(50); // Ускоренная загрузка
            }
        }

        /// <summary>
        /// Отображение прогресс-бара в консоли
        /// </summary>
        private void DrawProgressBar(int progress, int barLength = 30)
        {
            Console.Write("[");
            int completedLength = (int)(barLength * (progress / 100.0));

            for (int i = 0; i < barLength; i++)
            {
                if (i < completedLength)
                    Console.Write("█");
                else
                    Console.Write("░");
            }

            Console.Write($"] {progress,3}%");

            if (progress == 100)
                Console.Write(" ✅");

            Console.WriteLine();
        }

        /// <summary>
        /// Получение цвета для отображения прогресса
        /// </summary>
        private ConsoleColor GetProgressColor(int progress)
        {
            return progress switch
            {
                < 30 => ConsoleColor.Red,
                < 70 => ConsoleColor.Yellow,
                _ => ConsoleColor.Green
            };
        }

        /// <summary>
        /// Отображение текущего состояния всех загрузок
        /// </summary>
        public void DisplayStatus()
        {
            Console.WriteLine("\n" + new string('═', 70));
            Console.WriteLine("📊 ТЕКУЩИЙ СТАТУС ЗАГРУЗОК:");
            Console.WriteLine(new string('═', 70));

            lock (_lockObject)
            {
                foreach (var task in _tasks.Values.OrderBy(t => t.Id))
                {
                    string status = task.IsCompleted ? "✅ Завершено" : "🔄 В процессе";
                    string timeInfo = task.IsCompleted
                        ? $"{task.Duration.TotalSeconds:F1} сек"
                        : $"{task.Duration.TotalSeconds:F1} сек (активно)";

                    Console.WriteLine($"#{task.Id,2} {task.Name,-20} " +
                                      $"[{task.Progress,3}%] {status,-15} " +
                                      $"Время: {timeInfo}");
                }
            }

            Console.WriteLine(new string('═', 70));

            // Статистика потоков
            Console.WriteLine("\n🧵 ИНФОРМАЦИЯ О ПОТОКАХ:");
            Console.WriteLine($"   Активных потоков: {_threads.Values.Count(t => t.IsAlive)}");
            Console.WriteLine($"   Всего потоков: {_threads.Count}");
            Console.WriteLine($"   Завершённых задач: {_tasks.Values.Count(t => t.IsCompleted)}");
        }

        /// <summary>
        /// Ожидание завершения всех загрузок
        /// </summary>
        public void WaitForCompletion()
        {
            Console.WriteLine("\n⏳ Ожидание завершения всех загрузок...");

            while (_threads.Values.Any(t => t.IsAlive))
            {
                DisplayStatus();
                Thread.Sleep(2000); // Обновление статуса каждые 2 секунды

                // Очистка завершенных потоков
                var completedIds = _threads
                    .Where(kvp => !kvp.Value.IsAlive)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var id in completedIds)
                {
                    _threads.TryRemove(id, out _);
                }
            }

            Console.WriteLine("\n🎉 Все загрузки завершены!");
            DisplayFinalStatistics();
        }

        /// <summary>
        /// Отображение финальной статистики
        /// </summary>
        private void DisplayFinalStatistics()
        {
            Console.WriteLine("\n" + new string('═', 70));
            Console.WriteLine("📈 ФИНАЛЬНАЯ СТАТИСТИКА:");
            Console.WriteLine(new string('═', 70));

            var completedTasks = _tasks.Values.Where(t => t.IsCompleted).ToList();

            if (completedTasks.Any())
            {
                var fastest = completedTasks.OrderBy(t => t.Duration).First();
                var slowest = completedTasks.OrderByDescending(t => t.Duration).First();
                var averageTime = completedTasks.Average(t => t.Duration.TotalSeconds);

                Console.WriteLine($"   Всего загружено файлов: {completedTasks.Count}");
                Console.WriteLine($"   Среднее время загрузки: {averageTime:F1} сек");
                Console.WriteLine($"   Самая быстрая загрузка: {fastest.Name} ({fastest.Duration.TotalSeconds:F1} сек)");
                Console.WriteLine($"   Самая медленная загрузка: {slowest.Name} ({slowest.Duration.TotalSeconds:F1} сек)");
                Console.WriteLine($"   Общее время выполнения: {(DateTime.Now - completedTasks.Min(t => t.StartTime)).TotalSeconds:F1} сек");
            }

            Console.WriteLine(new string('═', 70));
        }

        /// <summary>
        /// Пауза всех загрузок
        /// </summary>
        public void PauseAllDownloads()
        {
            _isRunning = false;
            Console.WriteLine("\n⏸️ Все загрузки приостановлены");
        }

        /// <summary>
        /// Возобновление всех загрузок
        /// </summary>
        public void ResumeAllDownloads()
        {
            _isRunning = true;
            Console.WriteLine("\n▶️ Все загрузки возобновлены");
        }

        /// <summary>
        /// Принудительная остановка всех загрузок
        /// </summary>
        public void StopAllDownloads()
        {
            Console.WriteLine("\n🛑 Принудительная остановка всех загрузок...");
            PauseAllDownloads();

            foreach (var thread in _threads.Values)
            {
                if (thread.IsAlive)
                {
                    try
                    {
                        thread.Abort(); // Не рекомендуется в production коде
                    }
                    catch
                    {
                        // Игнорируем исключения при остановке
                    }
                }
            }

            // Очистка
            _threads.Clear();
            _tasks.Clear();

            Console.WriteLine("Все потоки остановлены");
        }

        /// <summary>
        /// Запуск загрузки в пуле потоков
        /// </summary>
        public void StartDownloadWithThreadPool(int taskId, string fileName)
        {
            Console.WriteLine($"\n🏊 Запуск загрузки через ThreadPool: {fileName}");

            var task = new DownloadTask(taskId, fileName);
            _tasks.TryAdd(taskId, task);

            ThreadPool.QueueUserWorkItem(state =>
            {
                var downloadTask = (DownloadTask)state;
                Console.WriteLine($"[ThreadPool] Начата загрузка: {downloadTask.Name}");

                for (int i = 0; i <= 100; i++)
                {
                    lock (_lockObject)
                    {
                        downloadTask.UpdateProgress(i);
                    }

                    Thread.Sleep(100); // Быстрая загрузка через ThreadPool

                    if (i % 20 == 0) // Вывод прогресса каждые 20%
                    {
                        lock (_lockObject)
                        {
                            Console.WriteLine($"[ThreadPool] {downloadTask.Name}: {i}%");
                        }
                    }
                }

                Console.WriteLine($"[ThreadPool] ✅ Загрузка завершена: {downloadTask.Name}");
            }, task);
        }
    }

    /// <summary>
    /// Главный класс программы
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Многопоточная система загрузки файлов";

            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("🚀 ПРАКТИЧЕСКАЯ РАБОТА №9: СОЗДАНИЕ И УПРАВЛЕНИЕ ПОТОКАМИ В .NET");
            Console.WriteLine("=".PadRight(70, '='));
            Console.WriteLine("📁 Симуляция прогресса загрузки файлов с использованием потоков\n");

            var manager = new DownloadManager();

            // Демонстрация 1: Параллельные загрузки
            Console.WriteLine("ДЕМОНСТРАЦИЯ 1: ПАРАЛЛЕЛЬНЫЕ ЗАГРУЗКИ");
            Console.WriteLine(new string('-', 50));
            manager.StartMultipleDownloads(3);

            // Периодический вывод статуса
            Thread statusThread = new Thread(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(3000);
                    manager.DisplayStatus();
                }
            });
            statusThread.Start();

            // Ожидание частичного завершения
            Thread.Sleep(8000);

            // Демонстрация 2: ThreadPool
            Console.WriteLine("\n\nДЕМОНСТРАЦИЯ 2: ИСПОЛЬЗОВАНИЕ THREADPOOL");
            Console.WriteLine(new string('-', 50));
            manager.StartDownloadWithThreadPool(100, "ThreadPool_File.dat");

            // Демонстрация 3: Управление потоками
            Console.WriteLine("\n\nДЕМОНСТРАЦИЯ 3: УПРАВЛЕНИЕ ПОТОКАМИ");
            Console.WriteLine(new string('-', 50));

            // Пауза и возобновление
            Thread.Sleep(2000);
            manager.PauseAllDownloads();
            Thread.Sleep(2000);
            manager.ResumeAllDownloads();

            // Ожидание завершения всех загрузок
            manager.WaitForCompletion();

            // Демонстрация 4: Продвинутые сценарии
            Console.WriteLine("\n\nДЕМОНСТРАЦИЯ 4: СЛОЖНЫЕ СЦЕНАРИИ С ПОТОКАМИ");
            Console.WriteLine(new string('-', 50));

            AdvancedThreadDemo();

            Console.WriteLine("\n" + "=".PadRight(70, '='));
            Console.WriteLine("🏁 ПРОГРАММА ЗАВЕРШЕНА");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Демонстрация продвинутых возможностей работы с потоками
        /// </summary>
        static void AdvancedThreadDemo()
        {
            Console.WriteLine("\n🧪 ПРОДВИНУТЫЕ ТЕХНИКИ РАБОТЫ С ПОТОКАМИ:\n");

            // 1. Потоки с параметрами
            Console.WriteLine("1. ПОТОКИ С ПАРАМЕТРАМИ:");
            var paramThread = new Thread((object param) =>
            {
                var data = (string)param;
                Console.WriteLine($"   Поток с параметром: {data}");
            });
            paramThread.Start("Hello from parameter thread!");
            paramThread.Join();

            // 2. Приоритеты потоков
            Console.WriteLine("\n2. ПРИОРИТЕТЫ ПОТОКОВ:");

            var highPriorityThread = new Thread(() =>
            {
                Console.WriteLine($"   Высокий приоритет: {Thread.CurrentThread.Priority}");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"   Высокий приоритет: шаг {i}");
                    Thread.Sleep(100);
                }
            })
            { Priority = ThreadPriority.Highest };

            var lowPriorityThread = new Thread(() =>
            {
                Console.WriteLine($"   Низкий приоритет: {Thread.CurrentThread.Priority}");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"   Низкий приоритет: шаг {i}");
                    Thread.Sleep(100);
                }
            })
            { Priority = ThreadPriority.Lowest };

            highPriorityThread.Start();
            lowPriorityThread.Start();

            highPriorityThread.Join();
            lowPriorityThread.Join();

            // 3. Фоновые потоки
            Console.WriteLine("\n3. ФОНОВЫЕ И ПОЛЬЗОВАТЕЛЬСКИЕ ПОТОКИ:");

            var backgroundThread = new Thread(() =>
            {
                Console.WriteLine($"   Фоновый поток: {Thread.CurrentThread.IsBackground}");
                Thread.Sleep(2000);
                Console.WriteLine($"   Фоновый поток завершен");
            })
            { IsBackground = true };

            var foregroundThread = new Thread(() =>
            {
                Console.WriteLine($"   Пользовательский поток: {Thread.CurrentThread.IsBackground}");
                Thread.Sleep(1000);
                Console.WriteLine($"   Пользовательский поток завершен");
            })
            { IsBackground = false };

            backgroundThread.Start();
            foregroundThread.Start();

            // Ждем только пользовательский поток
            foregroundThread.Join();
            Console.WriteLine($"   Основной поток продолжает работу (фоновый может быть прерван)");

            // 4. Monitor (альтернатива lock)
            Console.WriteLine("\n4. СИНХРОНИЗАЦИЯ С ПОМОЩЬЮ MONITOR:");

            object syncObject = new object();
            int sharedCounter = 0;

            Thread[] monitorThreads = new Thread[3];
            for (int i = 0; i < 3; i++)
            {
                monitorThreads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 5; j++)
                    {
                        bool lockTaken = false;
                        try
                        {
                            Monitor.Enter(syncObject, ref lockTaken);
                            sharedCounter++;
                            Console.WriteLine($"   Поток {Thread.CurrentThread.ManagedThreadId}: счетчик = {sharedCounter}");
                        }
                        finally
                        {
                            if (lockTaken)
                                Monitor.Exit(syncObject);
                        }
                        Thread.Sleep(50);
                    }
                });
                monitorThreads[i].Start();
            }

            foreach (var thread in monitorThreads)
                thread.Join();

            Console.WriteLine($"   Финальное значение счетчика: {sharedCounter}");

            // 5. AutoResetEvent для синхронизации
            Console.WriteLine("\n5. СИНХРОНИЗАЦИЯ С AutoResetEvent:");

            AutoResetEvent signal = new AutoResetEvent(false);
            Thread signalThread = new Thread(() =>
            {
                Console.WriteLine($"   Вспомогательный поток: ожидание сигнала...");
                signal.WaitOne();
                Console.WriteLine($"   Вспомогательный поток: сигнал получен!");
            });

            signalThread.Start();
            Thread.Sleep(1000);
            Console.WriteLine($"   Основной поток: отправка сигнала...");
            signal.Set();
            signalThread.Join();
        }
    }
}