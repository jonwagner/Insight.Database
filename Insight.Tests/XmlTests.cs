using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Insight.Database;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

#pragma warning disable 0649

namespace Insight.Tests
{
	[TestFixture]
	public class XmlTests : BaseDbTest
	{
		class Result
		{
			public Data Data;
			public string String;
			public XmlDocument XmlDocument;
			public XDocument XDocument;
		}

		[DataContract]
		class Data
		{
			[DataMember]
			public string Text;
		}

		#region SingleColumn Deserialization Tests
		[Test]
		public void XmlSingleColumnCanDeserializeToXmlDocument()
		{
			var list = _connection.QuerySql<XmlDocument>("SELECT CONVERT(xml, '<data/>')", new { });

			Assert.IsNotNull(list);
			var doc = list[0];
			Assert.IsNotNull(doc);
			Assert.AreEqual("<data />", doc.OuterXml);
		}

		[Test]
		public void XmlSingleColumnCanDeserializeToXDocument()
		{
			var list = _connection.QuerySql<XDocument>("SELECT CONVERT(xml, '<data/>')", new { });

			Assert.IsNotNull(list);
			var doc = list[0];
			Assert.IsNotNull(doc);
			Assert.AreEqual("<data />", doc.ToString());
		}

		[Test]
		public void XmlSingleColumnCanDeserializeToString()
		{
			var list = _connection.QuerySql<string>("SELECT CONVERT(xml, '<data/>')", new { });

			Assert.IsNotNull(list);
			var s = list[0];
			Assert.IsNotNull(s);
			Assert.AreEqual("<data />", s);
		}
		#endregion

		#region Xml Column Deserialization Tests
		[Test]
		public void XmlColumnCanDeserializeToString()
		{
			var list = _connection.QuerySql<Result>("SELECT String=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

			Assert.IsNotNull(list);
			var result = list[0];
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.String);
			Assert.AreEqual("<Data><Text>foo</Text></Data>", result.String);
		}

		[Test]
		public void XmlColumnCanDeserializeToXmlDocument()
		{
			var list = _connection.QuerySql<Result>("SELECT XmlDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

			Assert.IsNotNull(list);
			var result = list[0];
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.XmlDocument);
			Assert.AreEqual("<Data><Text>foo</Text></Data>", result.XmlDocument.OuterXml);
		}

		[Test]
		public void XmlColumnCanDeserializeToXDocument()
		{
			var list = _connection.QuerySql<Result>("SELECT XDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

			Assert.IsNotNull(list);
			var result = list[0];
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.XDocument);
			Assert.AreEqual("<Data>\r\n  <Text>foo</Text>\r\n</Data>", result.XDocument.ToString());
		}

		[Test]
		public void XmlColumnCanDeserializeToObject()
		{
			var list = _connection.QuerySql<Result>("SELECT Data=CONVERT(xml, '<XmlTests.Data xmlns=\"http://schemas.datacontract.org/2004/07/Insight.Tests\"><Text>foo</Text></XmlTests.Data>')", new { });

			Assert.IsNotNull(list);
			var result = list[0];
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Data);
			Assert.AreEqual("foo", result.Data.Text);
		}
		#endregion

		#region Serialization Tests
		[Test]
		public void XmlDocumentCanSerializeToXmlParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Xml xml) AS SELECT Xml=@Xml", transaction: t);

				// create a document
				XmlDocument doc = new XmlDocument();
				doc.LoadXml("<Data><Text>foo</Text></Data>");

				var list = _connection.Query<XmlDocument>("InsightTestProc", new { Xml = doc }, transaction: t);
				var data = list[0];
				Assert.IsNotNull(data);
				Assert.AreEqual(doc.OuterXml, data.OuterXml);
			}
		}

		[Test]
		public void XDocumentCanSerializeToXmlParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Xml xml) AS SELECT Xml=@Xml", transaction: t);

				// create a document
				XDocument doc = XDocument.Parse("<Data><Text>foo</Text></Data>");

				var list = _connection.Query<XDocument>("InsightTestProc", new { Xml = doc }, transaction: t);
				var data = list[0];
				Assert.IsNotNull(data);
				Assert.AreEqual(doc.ToString(), data.ToString());
			}
		}

		[Test]
		public void ObjectCanSerializeToXmlParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Xml xml) AS SELECT Data=@Xml", transaction: t);

				// create a document
				Data d = new Data()
				{
					Text = "foo"
				};

				var list = _connection.Query<Result>("InsightTestProc", new { Xml = d }, transaction: t);
				var data = list[0];
				Assert.IsNotNull(data);
				Assert.AreEqual(d.Text, data.Data.Text);
			}
		}

		[Test]
		public void StringCanSerializeToXmlParameter()
		{
			using (SqlTransaction t = _connection.BeginTransaction())
			{
				_connection.ExecuteSql("CREATE PROC InsightTestProc (@Xml xml) AS SELECT Xml=@Xml", transaction: t);

				// create a document
				string doc = "<Data><Text>foo</Text></Data>";

				var list = _connection.Query<string>("InsightTestProc", new { Xml = doc }, transaction: t);
				var data = list[0];
				Assert.IsNotNull(data);
				Assert.AreEqual(doc, data);
			}
		}
		#endregion
	}

	[TestFixture]
	public class XmlTVPTests : BaseDbTest
	{
		class Result
		{
			public Data Data;
		}

		[DataContract]
		class Data
		{
			[DataMember]
			public string Text;
		}

		#region SetUp and TearDown
		[TestFixtureSetUp]
		public override void SetUpFixture()
		{
			base.SetUpFixture();

			// clean up old stuff first
			CleanupObjects();

			_connection.ExecuteSql("CREATE TYPE [XmlDataTable] AS TABLE ([Data] [Xml])");
			_connection.ExecuteSql("CREATE PROCEDURE [XmlTestProc] @p [XmlDataTable] READONLY AS SELECT * FROM @p");
		}

		[TestFixtureTearDown]
		public override void TearDownFixture()
		{
			CleanupObjects();

			base.TearDownFixture();
		}

		private void CleanupObjects()
		{
			try
			{
				_connection.ExecuteSql("IF EXISTS (SELECT * FROM sys.objects WHERE name = 'XmlTestProc') DROP PROCEDURE [XmlTestProc]");
			}
			catch { }
			try
			{
				_connection.ExecuteSql("IF EXISTS (SELECT * FROM sys.types WHERE name = 'XmlDataTable') DROP TYPE [XmlDataTable]");
			}
			catch { }
		}
		#endregion

		[Test]
		public void XmlFieldCanBeSerializedInTVP()
		{
			Result r = new Result();
			r.Data = new Data();
			r.Data.Text = "foo";

			var list = _connection.Query<Result>("XmlTestProc", new { p = new List<Result>() { r } });
			var item = list[0];

			Assert.AreEqual(r.Data.Text, item.Data.Text);
		}
	}
}
