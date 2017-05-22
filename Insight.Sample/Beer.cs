using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Database.Sample
{
	class Beer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Flavor { get; set; }
		public decimal? OriginalGravity { get; set; }
		public string Details { get; set; }
		public int InStock { get; set; }
		public string Style { get; set; }

		public Beer()
		{
		}

		public Beer(string name)
		{
			Name = name;
		}
	}
}
