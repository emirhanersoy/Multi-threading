using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static long totalSum = 0;
    static object lockObject = new object();

    static void Main(string[] args)
    {
        string filePath = "output.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("İplik Sayısı\tSüre (ms)\tSüre (µs)\tSüre (ns)\tSonuç");
            Console.WriteLine("İplik Sayısı\tSüre (ms)\tSüre (µs)\t\tSüre (ns)\t\tSonuç\n");

            for (int threadCount = 1; threadCount <= 32; threadCount++)
            {
                long result = 0;
                totalSum = 0;

                Stopwatch stopwatch = Stopwatch.StartNew();
                Thread[] threads = new Thread[threadCount];
                int rangePerThread = 10_000_000 / threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    int start = i * rangePerThread + 1;
                    int end = (i == threadCount - 1) ? 10_000_000 : (i + 1) * rangePerThread;

                    threads[i] = new Thread(() => SumRange(start, end));
                    threads[i].Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }

                stopwatch.Stop();
                result = totalSum;

                double elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                double elapsedMicroseconds = (stopwatch.ElapsedTicks / (double)Stopwatch.Frequency) * 1_000_000;
                double elapsedNanoseconds = elapsedMicroseconds * 1_000;

                string output = $"{threadCount}\t\t{elapsedMilliseconds:F3}\t\t{elapsedMicroseconds:F3}\t\t{elapsedNanoseconds:F3}\t\t{result}";
                writer.WriteLine(output);
                Console.WriteLine(output);
            }
        }

        Console.WriteLine($"Çıktı {filePath} dosyasına kaydedildi.");
        Console.ReadKey();
    }

    static void SumRange(int start, int end)
    {
        long localSum = 0;
        for (int i = start; i <= end; i++)
        {
            localSum += i;
        }

        lock (lockObject)
        {
            totalSum += localSum;
        }
    }
}
