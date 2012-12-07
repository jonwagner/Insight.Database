using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Sample
{
    class PerformanceTest
    {
        public static void WithDuration(int runs, long milliseconds, Action action)
        {
            int[] iterations = new int[runs];
            for (int tests = 0; tests < iterations.Length; tests++)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                int i = 0;
                while (timer.ElapsedMilliseconds < milliseconds)
                {
                    action();
                    i++;
                }

                iterations[tests] = i;
            }

            for (int i = 0; i < iterations.Length; i++)
                Console.WriteLine("{0} iterations", iterations[i]);
            Console.WriteLine("{0} average", iterations.Average());
        }

        public static void WithDurationAndKeyWait(int runs, long milliseconds, Action action)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            Console.WriteLine("Running...");

            WithDuration(runs, milliseconds, action);
        }
    }
}
