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

	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2> : Results<T1>
	{
		/// <summary>
		/// Gets the second set of data returned from the database.
		/// </summary>
		public IList<T2> Set2 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 2) ? withGraphs[1] : null;
			Set2 = reader.ToList<T2>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 2) ? withGraphs[1] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set2 = await reader.ToListAsync<T2>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
	}

	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
	/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3> : Results<T1, T2>
	{
		/// <summary>
		/// Gets the third set of data returned from the database.
		/// </summary>
		public IList<T3> Set3 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 3) ? withGraphs[2] : null;
			Set3 = reader.ToList<T3>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 3) ? withGraphs[2] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set3 = await reader.ToListAsync<T3>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
	}

	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
	/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4> : Results<T1, T2, T3>
	{
		/// <summary>
		/// Gets the fourth set of data returned from the database.
		/// </summary>
		public IList<T4> Set4 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 4) ? withGraphs[3] : null;
			Set4 = reader.ToList<T4>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 4) ? withGraphs[3] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set4 = await reader.ToListAsync<T4>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
	}

	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
	/// <typeparam name="T3">The type of the data in the third set of data.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth set of data.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5> : Results<T1, T2, T3, T4>
	{
		/// <summary>
		/// Gets the fifth set of data returned from the database.
		/// </summary>
		public IList<T5> Set5 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 5) ? withGraphs[4] : null;
			Set5 = reader.ToList<T5>(withGraph);
		}

#if !NODBASYNC
		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		/// <param name="cancellationToken">The cancellationToken to use with the current operation.</param>
		/// <returns>A task representing the completion of the read operation.</returns>
		protected override async Task ReadAsync(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null)
		{
			Type withGraph = (withGraphs != null && withGraphs.Length >= 5) ? withGraphs[4] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set5 = await reader.ToListAsync<T5>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
	}
	#endregion
}