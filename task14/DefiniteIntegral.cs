using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double totalResult = 0.0;
        double range = b - a;
        double segmentWidth = range / threadsNumber;

        using Barrier barrier = new Barrier(threadsNumber + 1);

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;
            Thread thread = new Thread(() =>
            {
                double startX = a + threadIndex * segmentWidth;
                double endX = startX + segmentWidth;
                double localSum = 0.0;

                double currentX = startX;
                while (currentX < endX)
                {
                    double nextX = currentX + step;
                    if (nextX > endX) nextX = endX;

                    double y1 = function(currentX);
                    double y2 = function(nextX);
                    localSum += (y1 + y2) * (nextX - currentX) / 2.0;

                    currentX = nextX;
                }

                double initialValue;
                double newValue;
                do
                {
                    initialValue = totalResult;
                    newValue = initialValue + localSum;
                } 
                while (Interlocked.CompareExchange(ref totalResult, newValue, initialValue) != initialValue);

                barrier.SignalAndWait();
            });

            thread.Start();
        }

        barrier.SignalAndWait();
        return totalResult;
    }
}
