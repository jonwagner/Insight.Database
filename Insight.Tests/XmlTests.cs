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
using System.Data.Common;

#pragma warning disable 0649

namespace Insight.Tests
{
    [TestFixture]
    public class XmlTests : BaseTest
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
        public void BigXmlIsDeserializedProperly()
        {
            using (var c = Connection().OpenWithTransaction())
            {
                c.ExecuteSql("CREATE TABLE BigXml(stuff varchar(MAX))");
                c.ExecuteSql("INSERT INTO BigXml VALUES(@s)", new { s = new String('x', 10000) });

                var inner = (SqlConnection)c.InnerConnection;

                var result = inner.QueryXml("SELECT * FROM BigXml FOR XML AUTO", commandType: CommandType.Text, transaction: c);
            }
        }

        [Test]
        public void XmlSingleColumnCanDeserializeToXmlDocument()
        {
            var list = Connection().QuerySql<XmlDocument>("SELECT CONVERT(xml, '<data/>')", new { });

            Assert.IsNotNull(list);
            var doc = list[0];
            Assert.IsNotNull(doc);
            Assert.AreEqual("<data />", doc.OuterXml);
        }

        [Test]
        public void XmlSingleColumnCanDeserializeToXDocument()
        {
            var list = Connection().QuerySql<XDocument>("SELECT CONVERT(xml, '<data/>')", new { });

            Assert.IsNotNull(list);
            var doc = list[0];
            Assert.IsNotNull(doc);
            Assert.AreEqual("<data />", doc.ToString());
        }

        [Test]
        public void XmlSingleColumnCanDeserializeToString()
        {
            var list = Connection().QuerySql<string>("SELECT CONVERT(xml, '<data/>')", new { });

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
            var list = Connection().QuerySql<Result>("SELECT String=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

            Assert.IsNotNull(list);
            var result = list[0];
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.String);
            Assert.AreEqual("<Data><Text>foo</Text></Data>", result.String);
        }

        [Test]
        public void XmlColumnCanDeserializeToXmlDocument()
        {
            var list = Connection().QuerySql<Result>("SELECT XmlDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

            Assert.IsNotNull(list);
            var result = list[0];
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.XmlDocument);
            Assert.AreEqual("<Data><Text>foo</Text></Data>", result.XmlDocument.OuterXml);
        }

        [Test]
        public void XmlColumnCanDeserializeToXDocument()
        {
            var list = Connection().QuerySql<Result>("SELECT XDocument=CONVERT(xml, '<Data><Text>foo</Text></Data>')", new { });

            Assert.IsNotNull(list);
            var result = list[0];
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.XDocument);
            Assert.AreEqual(String.Format("<Data>{0}  <Text>foo</Text>{0}</Data>", Environment.NewLine), result.XDocument.ToString());
        }

        [Test]
        public void XmlColumnCanDeserializeToObjectSingle()
        {
            var result = Connection().SingleSql<Result>("SELECT Data=CONVERT(xml, '<XmlTests.Data xmlns=\"http://schemas.datacontract.org/2004/07/Insight.Tests\"><Text>foo</Text></XmlTests.Data>')", new { });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("foo", result.Data.Text);
        }

        [Test]
        public void XmlColumnCanDeserializeToObject()
        {
            var list = Connection().QuerySql<Result>("SELECT Data=CONVERT(xml, '<XmlTests.Data xmlns=\"http://schemas.datacontract.org/2004/07/Insight.Tests\"><Text>foo</Text></XmlTests.Data>')", new { });

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
            // create a document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Data><Text>foo</Text></Data>");

            var list = Connection().Query<XmlDocument>("ReflectXml", new { Xml = doc });
            var data = list[0];
            Assert.IsNotNull(data);
            Assert.AreEqual(doc.OuterXml, data.OuterXml);
        }

        [Test]
        public void XDocumentCanSerializeToXmlParameter()
        {
            // create a document
            XDocument doc = XDocument.Parse("<Data><Text>foo</Text></Data>");

            var list = Connection().Query<XDocument>("ReflectXml", new { Xml = doc });
            var data = list[0];
            Assert.IsNotNull(data);
            Assert.AreEqual(doc.ToString(), data.ToString());
        }

        [Test]
        public void ObjectCanSerializeToXmlParameter()
        {
            // create a document
            Data d = new Data()
            {
                Text = "foo"
            };

            var list = Connection().Query<Result>("ReflectXmlAsData", new { Xml = d });
            var data = list[0];
            Assert.IsNotNull(data);
            Assert.AreEqual(d.Text, data.Data.Text);
        }

        [Test]
        public void StringCanSerializeToXmlParameter()
        {
            // create a document
            string doc = "<Data><Text>foo</Text></Data>";

            var list = Connection().Query<string>("ReflectXml", new { Xml = doc });
            var data = list[0];
            Assert.IsNotNull(data);
            Assert.AreEqual(doc, data);
        }
        #endregion
    }

    [TestFixture]
    public class XmlTVPTests : BaseTest
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

        [Test]
        public void XmlFieldCanBeSerializedInTVP()
        {
            Result r = new Result();
            r.Data = new Data();
            r.Data.Text = "foo";

            var list = Connection().Query<Result>("ReflectXmlTable", new { p = new List<Result>() { r } });
            var item = list[0];

            Assert.AreEqual(r.Data.Text, item.Data.Text);
        }

        [Test]
        public void StringXmlCanBeSentAndReturnedAsStrings()
        {
            string s = "<xml>text</xml>";
            var input = new List<string>() { s };

            var list = Connection().Query<string>("ReflectXmlTableAsVarChar", new { p = input.Select(x => new { id = 1, data = x }).ToList() });
            var item = list[0];

            Assert.AreEqual(s, item);
        }
    }
}
