using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Insight.Database.Structure;

namespace Insight.Database.Structure
{
	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2> : Guardian<TChild, T1>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*2")]
		public T2 ParentId2 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId2 = (T2)reader[1];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2);
		}
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2, T3> : Guardian<TChild, T1, T2>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*3")]
		public T3 ParentId3 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId3 = (T3)reader[2];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2, ParentId3);
		}
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2, T3, T4> : Guardian<TChild, T1, T2, T3>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*4")]
		public T4 ParentId4 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId4 = (T4)reader[3];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2, ParentId3, ParentId4);
		}
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2, T3, T4, T5> : Guardian<TChild, T1, T2, T3, T4>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*5")]
		public T5 ParentId5 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId5 = (T5)reader[4];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2, ParentId3, ParentId4, ParentId5);
		}
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2, T3, T4, T5, T6> : Guardian<TChild, T1, T2, T3, T4, T5>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*6")]
		public T6 ParentId6 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId6 = (T6)reader[5];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2, ParentId3, ParentId4, ParentId5, ParentId6);
		}
	}

	/// <summary>
	/// Helps a child object find its parent.
	/// It knows the parent's ID by getting it from the recordset.
	/// </summary>
	/// <typeparam name="TChild">The type of the child.</typeparam>
	/// <typeparam name="T1">The type of the data in the first subobject.</typeparam>
	/// <typeparam name="T2">The type of the data in the second subobject.</typeparam>
	/// <typeparam name="T3">The type of the data in the third subobject.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth subobject.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth subobject.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth subobject.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh subobject.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
	public class Guardian<TChild, T1, T2, T3, T4, T5, T6, T7> : Guardian<TChild, T1, T2, T3, T4, T5, T6>
	{
		/// <summary>
		/// Gets or sets the ID of the parent. This is assumed to be the first column in the recordset.
		/// </summary>
		[Column("*7")]
		public T7 ParentId7 { get; set; }

		/// <inheritdoc/>
		public override void ReadCurrent(IDataReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			base.ReadCurrent(reader);
			ParentId7 = (T7)reader[6];
		}

		/// <inheritdoc/>
		public override object GetID()
		{
			return Tuple.Create(ParentId1, ParentId2, ParentId3, ParentId4, ParentId5, ParentId6, ParentId7);
		}
	}

}
