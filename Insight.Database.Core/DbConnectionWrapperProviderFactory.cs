using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database
{
	/// <summary>
	/// Implements a DbProviderFactory for a wrapped connection.
	/// </summary>
	/// <typeparam name="T">The type of DbProviderFactory to do the real implementation.</typeparam>
	/// <remarks>This was added for Glimpe support, which needs a provider factory for some reason.</remarks>
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "The implementation of DbProviderFactory is generated code")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "This class only implements certain members")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documenting the implementation of DbProviderFactory would be redundant without adding additional information.")]
	public class DbConnectionWrapperProviderFactory<T> : DbProviderFactory where T : DbProviderFactory
	{
		/// <summary>
		/// The singleton instance of the provider factory.
		/// </summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DbConnectionWrapperProviderFactory<T> Instance = new DbConnectionWrapperProviderFactory<T>();

		/// <summary>
		/// Gets or sets the implementing inner factory.
		/// </summary>
		private T InnerFactory { get; set; }

		/// <summary>
		/// Initializes a new instance of the DbConnectionWrapperProviderFactory class.
		/// </summary>
		public DbConnectionWrapperProviderFactory()
        {
            var field = typeof(T).GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new NotSupportedException("Provider doesn't have Instance property.");

            InnerFactory = (T)field.GetValue(null);
        }

		#region Implementation
		/// <inheritdoc/>
		public override bool CanCreateDataSourceEnumerator
		{
			get { return InnerFactory.CanCreateDataSourceEnumerator; }
		}

		/// <inheritdoc/>
		public override DbCommand CreateCommand()
		{
			return InnerFactory.CreateCommand();
		}

		/// <inheritdoc/>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return InnerFactory.CreateCommandBuilder();
		}

		/// <inheritdoc/>
		public override DbConnection CreateConnection()
		{
			return new DbConnectionWrapper(InnerFactory.CreateConnection());
		}

		/// <inheritdoc/>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return InnerFactory.CreateConnectionStringBuilder();
		}

		/// <inheritdoc/>
		public override DbDataAdapter CreateDataAdapter()
		{
			return InnerFactory.CreateDataAdapter();
		}

		/// <inheritdoc/>
		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return InnerFactory.CreateDataSourceEnumerator();
		}

		/// <inheritdoc/>
		public override DbParameter CreateParameter()
		{
			return InnerFactory.CreateParameter();
		}

#if !NO_DB_PROVIDER
		/// <inheritdoc/>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return InnerFactory.CreatePermission(state);
		}
#endif
#endregion
	}
}
