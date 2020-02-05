using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Insight.Tests.MsSqlClient.Cases
{
	/// <summary>
	/// Tests all of the cases for managing connection state.
	/// </summary>
	internal class ConnectionStateCase : IDisposable
	{
		public bool IsOpen;

		public IDbConnection Connection { get; private set; }

		public ConnectionStateCase(bool open)
		{
			IsOpen = open;
			Connection = new SqlConnection(BaseTest.ConnectionString);
			if (IsOpen)
				Connection.Open();
		}

		public void Dispose()
		{
			Connection.Dispose();
			GC.SuppressFinalize(this);
		}

		public static void ForEach(Action<IDbConnection> test)
		{
			foreach (var c in new List<ConnectionStateCase>() { new ConnectionStateCase(false), new ConnectionStateCase(true) })
				using (c)
				{
					c.VerifyPreCondition();
					test(c.Connection);
					c.VerifyPostCondition();
				}
		}

		private void VerifyPreCondition()
		{
			Assert.AreEqual(IsOpen, Connection.State == ConnectionState.Open);
		}

		private void VerifyPostCondition()
		{
			Assert.AreEqual(IsOpen, Connection.State == ConnectionState.Open);
		}
	}
}
