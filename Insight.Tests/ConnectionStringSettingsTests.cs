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
	/// <summary>
	/// Tests the behavior of the ConnectionStringSettings extensions.
	/// </summary>
	[TestFixture]
	public class ConnectionStringSettingsTests
	{
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConnectionShouldThrowArgumentNullExceptionOnNull()
		{
			ConnectionStringSettings settings = null;

			settings.Connection();
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void OpenShouldThrowArgumentNullExceptionOnNull()
		{
			ConnectionStringSettings settings = null;

			settings.Open();
		}
	}
}
