using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insight.Database;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Transactions;
using System.Data;
using System.Data.Common;

namespace Insight.Tests
{
	/// <summary>
	/// Base class to run database tests
	/// </summary>
	public class BaseDbTest
	{
		#region SetUp
		[TestFixtureSetUp]
		public virtual void SetUpFixture()
		{
			// open the test connection
			_connectionStringBuilder = new SqlConnectionStringBuilder();
			_connectionStringBuilder.IntegratedSecurity = true;
			_connectionStringBuilder.AsynchronousProcessing = true;
			_connection = _connectionStringBuilder.Open();
		}

		[TestFixtureTearDown]
		public virtual void TearDownFixture()
		{
			if (_connection.State != ConnectionState.Closed)
				_connection.Close();
		}

		[SetUp]
		public virtual void SetUp()
		{
			if (_connection.State != ConnectionState.Open)
				_connection.Open();
		}

		[TearDown]
		public virtual void TearDown()
		{
			if (_connection.State != ConnectionState.Closed)
				_connection.Close();
		}

		protected SqlConnection _sqlConnection { get { return _connection as SqlConnection; } }

		protected SqlConnectionStringBuilder _connectionStringBuilder;
		protected DbConnection _connection;
		protected TransactionScope _transaction;
		#endregion

		/// <summary>
		/// Execute some SQL that should clean something up. Catch exceptions so we clean up as much as possible.
		/// </summary>
		/// <param name="sql">The SQL to execute.</param>
		protected void Cleanup(string sql)
		{
			try
			{
				_connection.ExecuteSql(sql);
			}
			catch { }
		}
	}
}
