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
	/// <summary>
	/// Encapsulates multiple sets of data returned from the database.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first set of data.</typeparam>
	/// <typeparam name="T2">The type of the data in the second set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
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
		/// <returns>A task representing the completion of this operation.</returns>
		protected override async Task ReadAsync(IDbCommand command, IDataReader reader, Type[] withGraphs = null, CancellationToken? cancellationToken = null)
		{
			Type withGraph = (withGraphs != null && withGraphs.Length >= 5) ? withGraphs[4] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set5 = await reader.ToListAsync<T5>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6> : Results<T1, T2, T3, T4, T5>
	{
		/// <summary>
		/// Gets the sixth set of data returned from the database.
		/// </summary>
		public IList<T6> Set6 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 6) ? withGraphs[5] : null;
			Set6 = reader.ToList<T6>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 6) ? withGraphs[5] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set6 = await reader.ToListAsync<T6>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7> : Results<T1, T2, T3, T4, T5, T6>
	{
		/// <summary>
		/// Gets the seventh set of data returned from the database.
		/// </summary>
		public IList<T7> Set7 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 7) ? withGraphs[6] : null;
			Set7 = reader.ToList<T7>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 7) ? withGraphs[6] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set7 = await reader.ToListAsync<T7>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8> : Results<T1, T2, T3, T4, T5, T6, T7>
	{
		/// <summary>
		/// Gets the eighth set of data returned from the database.
		/// </summary>
		public IList<T8> Set8 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 8) ? withGraphs[7] : null;
			Set8 = reader.ToList<T8>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 8) ? withGraphs[7] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set8 = await reader.ToListAsync<T8>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Results<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		/// <summary>
		/// Gets the nineth set of data returned from the database.
		/// </summary>
		public IList<T9> Set9 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 9) ? withGraphs[8] : null;
			Set9 = reader.ToList<T9>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 9) ? withGraphs[8] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set9 = await reader.ToListAsync<T9>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		/// <summary>
		/// Gets the tenth set of data returned from the database.
		/// </summary>
		public IList<T10> Set10 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 10) ? withGraphs[9] : null;
			Set10 = reader.ToList<T10>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 10) ? withGraphs[9] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set10 = await reader.ToListAsync<T10>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		/// <summary>
		/// Gets the eleventh set of data returned from the database.
		/// </summary>
		public IList<T11> Set11 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 11) ? withGraphs[10] : null;
			Set11 = reader.ToList<T11>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 11) ? withGraphs[10] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set11 = await reader.ToListAsync<T11>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
	{
		/// <summary>
		/// Gets the twelfth set of data returned from the database.
		/// </summary>
		public IList<T12> Set12 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 12) ? withGraphs[11] : null;
			Set12 = reader.ToList<T12>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 12) ? withGraphs[11] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set12 = await reader.ToListAsync<T12>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		/// <summary>
		/// Gets the thirteenth set of data returned from the database.
		/// </summary>
		public IList<T13> Set13 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 13) ? withGraphs[12] : null;
			Set13 = reader.ToList<T13>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 13) ? withGraphs[12] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set13 = await reader.ToListAsync<T13>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		/// <summary>
		/// Gets the fourteenth set of data returned from the database.
		/// </summary>
		public IList<T14> Set14 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 14) ? withGraphs[13] : null;
			Set14 = reader.ToList<T14>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 14) ? withGraphs[13] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set14 = await reader.ToListAsync<T14>(withGraph, cancellationToken).ConfigureAwait(false);
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
	/// <typeparam name="T6">The type of the data in the sixth set of data.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh set of data.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth set of data.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth set of data.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth set of data.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh set of data.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth set of data.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth set of data.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth set of data.</typeparam>
	/// <typeparam name="T15">The type of the data in the fifteenth set of data.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance"), SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "The classes are related by implementing multiple generic signatures.")]
	public class Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Results<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
	{
		/// <summary>
		/// Gets the fifteenth set of data returned from the database.
		/// </summary>
		public IList<T15> Set15 { get; private set; }

		/// <summary>
		/// Reads the contents from an IDataReader.
		/// </summary>
		/// <param name="command">The command that generated the result set.</param>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="withGraphs">The object graphs to use to deserialize the objects.</param>
		public override void Read(IDbCommand command, IDataReader reader, Type[] withGraphs = null)
		{
			base.Read(command, reader, withGraphs);

			Type withGraph = (withGraphs != null && withGraphs.Length >= 15) ? withGraphs[14] : null;
			Set15 = reader.ToList<T15>(withGraph);
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
			Type withGraph = (withGraphs != null && withGraphs.Length >= 15) ? withGraphs[14] : null;

			await base.ReadAsync(command, reader, withGraphs).ConfigureAwait(false);

			Set15 = await reader.ToListAsync<T15>(withGraph, cancellationToken).ConfigureAwait(false);
		}
#endif
    }


}


