using NUnit.Framework;
using NUnit.Framework.Legacy;

#pragma warning disable 0649

namespace Insight.Tests.MsSqlClient.Cases
{
    /// <summary>
    /// Standard In/Out parameters structure.
    /// </summary>
    internal class InOutParameters
    {
        public int In;
        public int Out1;

        public const string ProcName = "ProcWithOutputParameters";

        public void Verify(OutParameters output)
        {
            ClassicAssert.AreEqual(In, Out1);
            output.Verify(In);
        }
    }

    /// <summary>
    /// Standard output parameters structure.
    /// </summary>
    internal class OutParameters
    {
        public int Out2;
        public int Return_Value;

        public void Verify(int input)
        {
            ClassicAssert.AreEqual(input, Out2);
            ClassicAssert.AreEqual(input, Return_Value);
        }
    }
}
