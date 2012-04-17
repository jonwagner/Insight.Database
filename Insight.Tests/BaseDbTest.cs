using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Transactions;
using System.Data;

namespace Insight.Tests
{
	/// <summary>
	/// Base class to run database tests
	/// </summary>
	public class BaseDbTest
	{
		#region SetUp
		[TestFixtureSetUp]
		public virtual void SetUpFixture ()
		{
			// open the test connection
			SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder ();
			sb.IntegratedSecurity = true;
			sb.AsynchronousProcessing = true;
			_connection = new SqlConnection (sb.ConnectionString);

			if (_connection.State != ConnectionState.Open)
				_connection.Open ();
		}

		[TestFixtureTearDown]
		public virtual void TearDownFixture ()
		{
			if (_connection.State != ConnectionState.Closed)
				_connection.Close ();
		}

		[SetUp]
		public virtual void SetUp ()
		{
			if (_connection.State != ConnectionState.Open)
				_connection.Open ();
		}

		[TearDown]
		public virtual void TearDown ()
		{
			if (_connection.State != ConnectionState.Closed)
				_connection.Close ();
		}

		protected SqlConnection _connection;
		protected TransactionScope _transaction;
		#endregion
	}
}
