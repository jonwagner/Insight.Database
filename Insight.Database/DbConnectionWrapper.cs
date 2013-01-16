using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Wraps an IDbConnection with a retry strategy to handle transient exceptions with retry logic.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of IDbConnection is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of IDbConnection would be redundant without adding additional information.")]
	public class DbConnectionWrapper : DbConnection
	{
		#region Private Members
		/// <summary>
		/// Gets the inner connection to use to execute the database commands.
		/// </summary>
		internal DbConnection InnerConnection { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the DbConnectionWrapper class.
		/// A default retry strategy is used.
		/// </summary>
		/// <param name="innerConnection">The inner connection to wrap.</param>
		public DbConnectionWrapper(DbConnection innerConnection)
		{
			InnerConnection = innerConnection;
		}
		#endregion

		#region Core Implementation Methods
		/// <summary>
		/// Opens the database connection with retry.
		/// </summary>
		public override void Open()
		{
			InnerConnection.Open();
		}

		/// <summary>
		/// Creates a DbCommand for calls to the database.
		/// </summary>
		/// <returns>A ReliableCommand.</returns>
		protected override DbCommand CreateDbCommand()
		{
			return InnerConnection.CreateCommand();
		}
		#endregion

		#region IDbConnection Implementation
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return InnerConnection.BeginTransaction(isolationLevel);
		}

		protected override bool CanRaiseEvents
		{
			get
			{
				return false;
			}
		}

		public override void ChangeDatabase(string databaseName)
		{
			InnerConnection.ChangeDatabase(databaseName);
		}

		public override void Close()
		{
			InnerConnection.Close();
		}

		public override string ConnectionString
		{
			get
			{
				return InnerConnection.ConnectionString;
			}

			set
			{
				InnerConnection.ConnectionString = value;
			}
		}

		public override string ServerVersion
		{
			get { return InnerConnection.ServerVersion; }
		}

		public override string DataSource
		{
			get { return InnerConnection.DataSource; }
		}

		public override int ConnectionTimeout
		{
			get { return InnerConnection.ConnectionTimeout; }
		}

		public override string Database
		{
			get { return InnerConnection.Database; }
		}

		public override ConnectionState State
		{
			get { return InnerConnection.State; }
		}

		public override DataTable GetSchema()
		{
			return InnerConnection.GetSchema();
		}

		public override DataTable GetSchema(string collectionName)
		{
			return InnerConnection.GetSchema(collectionName);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return InnerConnection.GetSchema(collectionName, restrictionValues);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				InnerConnection.Dispose();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		#endregion
	}
}
