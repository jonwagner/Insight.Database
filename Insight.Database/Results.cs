using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Insight.Database
{
	#region Classes Without Output Parameters
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
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public virtual void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			SaveCommandForOutputs(command);
		}

#if NODBASYNC
		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <typeparam name="T">The type to cast the results to.</typeparam>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use with the current operation.</param>
		/// <returns>A task representing the completion of this operation.</returns>
		public Task<T> ReadAsync<T>(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null) where T : Results
		{
			CancellationToken ct = (cancellationToken != null) ? cancellationToken.Value : CancellationToken.None;
			ct.ThrowIfCancellationRequested();

			Read(command, reader, withGraphs);

			return Helpers.FromResult((T)this);
		}
#else
		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <typeparam name="T">The type to cast the results to.</typeparam>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use with the current operation.</param>
		/// <returns>A task representing the completion of this operation.</returns>
		public async Task<T> ReadAsync<T>(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null) where T : Results
		{
			await ReadAsync(command, reader, withGraphs, cancellationToken).ConfigureAwait(false);

			return (T)this;
		}

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use with the current operation.</param>
		/// <returns>A task representing the completion of this operation.</returns>
		protected virtual Task ReadAsync(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null)
		{
			SaveCommandForOutputs(command);

			return Helpers.FalseTask;
		}
#endif

		/// <summary>
		/// Saves the command so that the output parameters can be read if necessary.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		private void SaveCommandForOutputs(IDbCommand command)
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
		/// Gets the first set of data returned from the database.
		/// </summary>
		public IList<T1> Set1 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 1) ? withGraphs[0] : null;
			Set1 = reader.ToList<T1>(withGraph);
		}

#if !NODBASYNC
		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use with the current operation.</param>
		/// <returns>A task representing the completion of this operation.</returns>
		protected override async Task ReadAsync(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null)
		{
			Type withGraph = (withGraphs != null && withGraphs.Length >= 1) ? withGraphs[0] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set1 = await reader.ToListAsync<T1>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
	}
	#endregion
}