using ScottPlot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laborki5
{
    internal class Program
    {

        internal interface IFunction
        {
            string Name { get; }
            double GetY(double x);
        }

        internal class Function1 : IFunction
        {
            public string Name => "y = 2x + 2x^2";

            public double GetY(double x) => 2 * x + 2 * Math.Pow(x, 2);
        }

        internal class Function2 : IFunction
        {
            public string Name => "y = 2x^2 + 3";

            public double GetY(double x) => 2 * Math.Pow(x, 2) + 3;
        }

        internal class Function3 : IFunction
        {
            public string Name => "y = 3x^2 + 2x - 3";

            public double GetY(double x) => 3 * Math.Pow(x, 2) + 2 * x - 3;
        }

        internal static class Integration
        {
            public static double CalculateRange(IFunction function, double start, double end, int intervals)
            {
                double step = (end - start) / intervals;
                double result = 0;

                for (int i = 0; i < intervals; i++)
                {
                    double x1 = start + i * step;
                    double x2 = x1 + step;
                    result += (function.GetY(x1) + function.GetY(x2)) / 2 * step;
                }

                return result;
            }
        }

        public static void RunParallelPartitions(IFunction function, double start, double end, int totalIntervals, int partitionCount)
        {

            try
            {
                var partitioner = Partitioner.Create(0, totalIntervals, totalIntervals / partitionCount);

                double totalResult = 0;
                object lockObject = new object();

                var plotThread = new Thread(() => Ploter(function, start, end));
                plotThread.Start();


                Parallel.ForEach(partitioner, (range) =>
                {
                    double localResult = Integration.CalculateRange(
                        function,
                        start + (range.Item1 * (end - start) / totalIntervals),
                        start + (range.Item2 * (end - start) / totalIntervals),
                        range.Item2 - range.Item1
                    );

                    lock (lockObject)
                    {
                        totalResult += localResult;
                    }


                    Console.WriteLine($"Task {Task.CurrentId}: Calculated range {range.Item1} to {range.Item2} -> Partial result: {localResult}");
                });

                plotThread.Join();

                Console.WriteLine($"Total integration result: {totalResult}");
                Console.ReadLine();
            } 
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("Invalid data");
            }
            finally
            {
                Console.ReadLine();
            }
            
        }

        public static void Ploter(IFunction function, double start, double end)
        {

            while (true)
            {
                Console.Write("Enter the file name to save the plot (e.g., plot.png): ");
                string fileName = Console.ReadLine();

                try
                {
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        throw new ArgumentException("File name cannot be empty.");
                    }

                    if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        fileName += ".png";
                    }

                    string fullPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                    int points = 100;

                    double[] dataX = Enumerable.Range(0, points).Select(i => start + i * (end - start) / points).ToArray();
                    double[] dataY = dataX.Select(x => function.GetY(x)).ToArray();

                    var plt = new ScottPlot.Plot(400, 300);

                    plt.AddScatter(dataX, dataY);
                    fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

                    using (var bmp = plt.GetBitmap())
                    {
                        bmp.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                  
                    var p = new System.Diagnostics.Process();


                    Console.WriteLine($"Current working directory: {Environment.CurrentDirectory}");
                    Console.WriteLine("Plot saved successfully.");

                    p.StartInfo = new System.Diagnostics.ProcessStartInfo(fileName)
                    {
                        UseShellExecute = true
                    };

                    p.Start();

                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Error: You do not have permission to save the file in the specified directory. Try another location.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    Console.WriteLine("Error: A GDI+ error occurred. Ensure the file is not open or locked by another process.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}. Please try again.");
                }

            }

           
        }

        static void Main(string[] args)
        {
            // RunParallelPartitions();
            IFunction function = new Function2();

            double start = 1;
            double end = 35;
            int intervals = 10;
            int partitionCount = 10;

            RunParallelPartitions(function ,start, end, intervals, partitionCount);
        }
    }
}
       
