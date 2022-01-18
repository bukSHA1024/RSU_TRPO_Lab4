using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4_mag
{
    internal class Program
    {
        private static void Main()
        {
            int size;
            try
            {
                size = ReadUserInput();
            }
            catch
            {
                return;
            }

            var (vectorA, vectorB) = GenerateData(size);
            PrintData(vectorA, nameof(vectorA));
            PrintData(vectorB, nameof(vectorB));

            // Multiple Thread Smart
            Console.WriteLine("(From Lab 3) Multiple Thread Smart results.");
            var watch = Stopwatch.StartNew();
            var result = CalculateFunctionMultipleThreadSmart(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Multiple thread Smart, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Concurrent access with Mutex
            Console.WriteLine("Mutex.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionMutex(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Mutex, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Concurrent access with Mutex
            Console.WriteLine("Monitor.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionMonitor(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Monitor, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();
        }

        #region Investigated functions

        private static int[] CalculateFunctionMultipleThreadSmart(int[] vectorA, int[] vectorB)
        {
            var firstMultiplyTask = Task.Run(() => Multiply(vectorB, 21));
            var secondMultiplyTask = Task.Run(() => Multiply(vectorA, 3));

            Task.WaitAll(firstMultiplyTask, secondMultiplyTask);

            var firstMultiplyResult = firstMultiplyTask.Result;
            var secondMultiplyResult = secondMultiplyTask.Result;
            var finalResult = Task.Run(() => Sum(firstMultiplyResult, secondMultiplyResult)).Result;
            return finalResult;
        }

        private static int[] CalculateFunctionMutex(int[] vectorA, int[] vectorB)
        {
            // Check input length
            var lengthA = vectorA.Length;
            var lengthB = vectorB.Length;
            if (lengthA != lengthB)
                throw new ArgumentException("Input vectors must have same size!");

            // Initializing shared resource
            var result = new MutexArray(lengthA);

            // Starting two threads
            var firstMultiplyTask = Task.Run(() => MultiplyAndAddResult(vectorB, 21, result));
            var secondMultiplyTask = Task.Run(() => MultiplyAndAddResult(vectorA, 3, result));

            // Waiting until task is finished
            Task.WaitAll(firstMultiplyTask, secondMultiplyTask);

            return result.GetData();
        }

        private static int[] CalculateFunctionMonitor(int[] vectorA, int[] vectorB)
        {
            // Check input length
            var lengthA = vectorA.Length;
            var lengthB = vectorB.Length;
            if (lengthA != lengthB)
                throw new ArgumentException("Input vectors must have same size!");

            // Initializing shared resource
            var result = new MonitorArray(lengthA);

            // Starting two threads
            var firstMultiplyTask = Task.Run(() => MultiplyAndAddResult(vectorB, 21, result));
            var secondMultiplyTask = Task.Run(() => MultiplyAndAddResult(vectorA, 3, result));

            // Waiting until task is finished
            Task.WaitAll(firstMultiplyTask, secondMultiplyTask);

            return result.GetData();
        }

        #endregion

        #region MathOperationsFast

        private static int[] Sum(int[] first, int[] second)
        {
            if (first.Length != second.Length)
                throw new ArgumentException("Pass lists with the same size!");

            var zippedLists = first.Zip(second, (f, s) => new { First = f, Second = s });

            return zippedLists.Select(item => item.First + item.Second).ToArray();
        }

        private static int[] Multiply(int[] vector, int k)
        {
            return vector.Select(n => n * k).ToArray();
        }

        private static void MultiplyAndAddResult(int[] vector, int k, IThreadSafeArray result)
        {
            var temporaryResult = vector.Select(n => n * k).ToList();
            for (var index = 0; index < temporaryResult.Count; index++)
            {
                result.AddToValueByIndex(index, temporaryResult[index]);
            }
        }

        #endregion

        #region Print

        private static void PrintData(IEnumerable<int> enumerable, string header)
        {
            Console.Write($"{header}: ");
            foreach (var item in enumerable) Console.Write($"{item} ");
            Console.WriteLine();
        }

        #endregion

        #region Initialization

        private static (int[], int[]) GenerateData(int size)
        {
            var rnd = new Random();
            var vectorA = Enumerable.Range(0, size).Select(_ => rnd.Next(1, 20)).ToArray();
            var vectorB = Enumerable.Range(0, size).Select(_ => rnd.Next(1, 20)).ToArray();

            return (vectorA, vectorB);
        }

        private static int ReadUserInput()
        {
            while (true)
            {
                Console.WriteLine("Enter q to exit.");
                Console.Write("Enter size of vectors: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var size))
                {
                    if (size <= 0)
                    {
                        Console.WriteLine("Size must be positive integer.");
                        continue;
                    }

                    return size;
                }

                if (input == "q")
                    throw new Exception();
            }
        }

        #endregion
    }
}
