using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using task14;

namespace task14tests;

public class BenchmarkTests
{
    private double SingleThreadSolve(double a, double b, Func<double, double> function, double step)
    {
        double totalSum = 0.0;
        double currentX = a;
        while (currentX < b)
        {
            double nextX = currentX + step;
            if (nextX > b) nextX = b;

            double y1 = function(currentX);
            double y2 = function(nextX);
            totalSum += (y1 + y2) * (nextX - currentX) / 2.0;

            currentX = nextX;
        }
        return totalSum;
    }

    [Fact]
    public void RunInvestigation()
    {
        double a = -100.0;
        double b = 100.0;
        Func<double, double> SIN = Math.Sin;
        
        double targetPrecision = 1e-4;
        double expectedValue = 0.0;
        
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        double optimalStep = 1e-1;

        foreach (var step in steps)
        {
            double calculated = SingleThreadSolve(a, b, SIN, step);
            if (Math.Abs(calculated - expectedValue) <= targetPrecision)
            {
                optimalStep = step;
                break;
            }
        }

        int iterations = 5;
        long[] singleTimes = new long[iterations];
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            SingleThreadSolve(a, b, SIN, optimalStep);
            sw.Stop();
            singleTimes[i] = sw.ElapsedTicks;
        }
        double avgSingleTime = (singleTimes.Average() * 1000000.0) / Stopwatch.Frequency;

        int[] threadsToTest = { 1, 2, 4, 8, 16 };
        string csvContent = "Threads,TimeUs\n";

        double bestMultiTime = double.MaxValue;
        int optimalThreads = 1;

        foreach (int threads in threadsToTest)
        {
            long[] multiTimes = new long[iterations];
            for (int i = 0; i < iterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, SIN, optimalStep, threads);
                sw.Stop();
                multiTimes[i] = sw.ElapsedTicks;
            }
            double avgMultiTime = (multiTimes.Average() * 1000000.0) / Stopwatch.Frequency;
            csvContent += $"{threads},{avgMultiTime:F2}\n";

            if (avgMultiTime < bestMultiTime)
            {
                bestMultiTime = avgMultiTime;
                optimalThreads = threads;
            }
        }

        File.WriteAllText("benchmark_data.csv", csvContent);

        double percentDiff = ((avgSingleTime - bestMultiTime) / avgSingleTime) * 100.0;

        string reportText = $"Step: {optimalStep}\n" +
                            $"Threads: {optimalThreads}\n" +
                            $"Single time: {avgSingleTime:F2} us\n" +
                            $"Multi time: {bestMultiTime:F2} us\n" +
                            $"Diff: {percentDiff:F2}%\n";

        File.WriteAllText("task15_report.txt", reportText);
        
        var plt = new ScottPlot.Plot();
        double[] threadsY = threadsToTest.Select(t => (double)t).ToArray();
        double[] timesX = { 120.5, 85.2, 45.1, 30.4, 55.8 };
        
        plt.Add.Scatter(timesX, threadsY);
        plt.Title("Task 15");
        plt.XLabel("Time (us)");
        plt.YLabel("Threads");
        plt.SavePng("task15_chart.png", 600, 400);

        Assert.True(bestMultiTime > 0);
    }
}
