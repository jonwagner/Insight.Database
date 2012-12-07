using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Insight.Database;
using Insight.Database.CodeGenerator;
using System.Diagnostics;

namespace Insight.Tests
{
    /// <summary>
    /// Holds test cases that can be used to test the performance of pieces of the system.
    /// </summary>
    [TestFixture]
    class PerformanceTests : BaseDbTest
    {
        [Test, Ignore]
        public void PerfTestSchemaMappingIdentity()
        {
            var reader = _connection.GetReaderSql("SELECT 1, 'two', 3.0, 'four', 'five', 6, 7, 8");

            PerfTestWithDuration(3, 1 * 1000, () =>
            {
                Dictionary<SchemaMappingIdentity, int> foo = new Dictionary<SchemaMappingIdentity, int>();
                foo.Add(new SchemaMappingIdentity(reader, typeof(TestDataClasses.TestData), null, SchemaMappingType.NewObject), 1);
                int i;
                foo.TryGetValue(new SchemaMappingIdentity(reader, typeof(TestDataClasses.TestData), null, SchemaMappingType.NewObject), out i);
            });
        }

        private void PerfTestWithDuration(int runs, long milliseconds, Action action)
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
    }
}
