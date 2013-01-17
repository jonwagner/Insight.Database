using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Insight.Tests
{
	[TestFixture]
	public class InterfaceTests : BaseDbTest
	{
		#region Test 1
		interface ITest1
		{
		}

		[Test]
		public void InterfaceIsGenerated()
		{
			ITest1 i = _connection.As<ITest1>();
		}
		#endregion
	}
}
