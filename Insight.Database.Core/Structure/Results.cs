using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database
{
	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results
	{
		#region Properties
		/// <summary>
		/// Contains the results of the output parameters of the query.
		/// </summary>
		private Lazy<dynamic> _outputs;

		/// <summary>
		/// Gets the outputs of the query.
		/// </summary>
		public dynamic Outputs { get { return _outputs.Value; } }
		#endregion

		/// <summary>
		/// Reads the records from the data reader.
		/// </summary>
		/// <param name="command">The command that was executed.</param>
		/// <param name="reader">The reader to read from.</param>
		public virtual void Read(IDbCommand command, IDataReader reader)
		{
			SaveCommandForOutputs(command);
		}

		/// <summary>
		/// Reads the records from the data reader asynchronously.
		/// </summary>
		/// <param name="command">The command that was executed.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
		/// <returns>A task representing the completion of the read.</returns>
		public virtual Task ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
		{
			SaveCommandForOutputs(command);
			return Helpers.FromResult(true);
		}

		/// <summary>
		/// Saves the command so that the output parameters can be read if necessary.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		internal void SaveCommandForOutputs(IDbCommand command)
		{
			_outputs = new Lazy<dynamic>(() => command.OutputParameters());
		}
	}

	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1> : Results
	{
		/// <summary>
		/// Constructs an instance of the Results class.
		/// </summary>
		public Results()
		{
		}

		/// <summary>
		/// Constructs an instance of the Results class.
		/// </summary>
		/// <param name="set1">The value of Set1.</param>
		public Results(IList<T1> set1)
		{
			Set1 = set1;
		}

		/// <summary>
		/// Gets the first set of data returned from the database.
		/// </summary>
		public IList<T1> Set1 { get; internal set; }

		/// <summary>
		/// Gets the default query reader for this class.
		/// </summary>
		/// <returns>A query reader that can read this class.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static IQueryReader GetReader()
		{
			return ResultsReader<T1>.Default;
		}

		/// <inheritdoc/>
		public override void Read(IDbCommand command, IDataReader reader)
		{
			ResultsReader<T1>.Default.Read(command, this, reader);
		}

		/// <inheritdoc/>
		public override Task ReadAsync(IDbCommand command, IDataReader reader, CancellationToken cancellationToken)
		{
			return ResultsReader<T1>.Default.ReadAsync(command, this, reader, cancellationToken);
		}
	}
}
