using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Mapping
{
	/// <summary>
	/// Allows for configuration of both the Parameters and Tables mappings at the same time.
	/// </summary>
	class DualMappingCollection : MappingCollection<IDualMapper>
	{
		/// <summary>
		/// The parameters submapping.
		/// </summary>
		private MappingCollection<IParameterMapper> _parameters;

		/// <summary>
		/// The tables submapping.
		/// </summary>
		private MappingCollection<IColumnMapper> _tables;

		/// <summary>
		/// Initializes a new instance of the DualMappingCollection class
		/// </summary>
		/// <param name="parameters">The paremeters submapping.</param>
		/// <param name="tables">The tables submapping.</param>
		public DualMappingCollection(MappingCollection<IParameterMapper> parameters, MappingCollection<IColumnMapper> tables) : base(BindChildrenFor.All)
		{
			_parameters = parameters;
			_tables = tables;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> EnableChildBinding<T>()
		{
			_parameters.EnableChildBinding<T>();
			_tables.EnableChildBinding<T>();
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> DisableChildBinding<T>()
		{
			_parameters.DisableChildBinding<T>();
			_tables.DisableChildBinding<T>();
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> ResetChildBinding()
		{
			_parameters.ResetChildBinding();
			_tables.ResetChildBinding();
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> AddTransform(IMappingTransform transform)
		{
			_parameters.AddTransform(transform);
			_tables.AddTransform(transform);
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> ResetTransforms()
		{
			_parameters.ResetTransforms();
			_tables.ResetTransforms();
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> AddMapper(IDualMapper mapper)
		{
			_parameters.AddMapper(mapper);
			_tables.AddMapper(mapper);
			return this;
		}

		/// <inheritdoc/>
		public override MappingCollection<IDualMapper> ResetMappers()
		{
			_parameters.ResetMappers();
			_tables.ResetMappers();
			return this;
		}
	}
}
