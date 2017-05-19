using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
using System.Configuration;

namespace Insight.Tests
{
#if !NO_CONNECTION_SETTINGS
	/// <summary>
	/// Tests the behavior of the ConnectionStringSettings extensions.
	/// </summary>
	[TestFixture]
	public class ConnectionStringSettingsTests
	{
		[Test]
		public void ConnectionShouldThrowArgumentNullExceptionOnNull()
		{
			ConnectionStringSettings settings = null;

			Assert.Throws<ArgumentNullException>(() => settings.Connection());
		}

		[Test]
		public void OpenShouldThrowArgumentNullExceptionOnNull()
		{
			ConnectionStringSettings settings = null;

			Assert.Throws<ArgumentNullException>(() => settings.Open());
		}
	}
#endif
}
