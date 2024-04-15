using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests.Cases
{
	/// <summary>
	/// Standard In/Out parameters structure.
	/// </summary>
	class InOutParameters
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
	class OutParameters
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
