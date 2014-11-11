using Insight.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Tests
{
    /// <summary>
    /// Tests to make sure that Insight can construct objects.
    /// </summary>
    [TestFixture]
    public class ConstructorTests : BaseTest
    {
        #region NonDefault Constructor
        public class ClassWithNonDefaultConstructor
        {
            // note that this has a readonly field
            public readonly int A;

            public ClassWithNonDefaultConstructor(int a)
            {
                A = a;
            }
        }

        [Test]
        public void TestClassWithNonDefaultConstructor()
        {
            var a = Connection().QuerySql<ClassWithNonDefaultConstructor>("SELECT A=3").First();
            Assert.AreEqual(3, a.A);
        }
        #endregion

        #region Secondary Properties
        public class ClassWithSecondaryProperties
        {
            // note that this has a readonly field
            public readonly int A;

            // this is set after construction
            public int B;

            public ClassWithSecondaryProperties(int a)
            {
                A = a;
            }
        }

        [Test]
        public void TestClassWithSecondaryProperties()
        {
            var a = Connection().QuerySql<ClassWithSecondaryProperties>("SELECT A=3, B=4").First();
            Assert.AreEqual(3, a.A);
            Assert.AreEqual(3, a.B);
        }
        #endregion

        #region Use Constructor Attribute
        public class ConstructorWithAttribute
        {
            // note that this has a readonly field
            public readonly int A;

            public ConstructorWithAttribute()
            {
            }

            [SqlConstructor]
            public ConstructorWithAttribute(int a)
            {
                A = a;
            }
        }

        [Test]
        public void TestConstructorWithAttribute()
        {
            var a = Connection().QuerySql<ConstructorWithAttribute>("SELECT A=3").First();
            Assert.AreEqual(3, a.A);
        }
        #endregion

        #region ColumnMapping
        public class ColumnMapping
        {
            // note that this has a readonly field
            [Column("foo")]
            public readonly int A;

            public ColumnMapping(int a)
            {
                A = a;
            }
        }

        [Test]
        public void TestColumnMapping()
        {
            var a = Connection().QuerySql<ColumnMapping>("SELECT Foo=3").First();
            Assert.AreEqual(3, a.A);
        }
        #endregion

        #region ParentWithChildren
        public class MyParent
        {
            public ClassWithNonDefaultConstructor Child;
        }

        [Test]
        public void TestParentWithChildren()
        {
            var a = Connection().QuerySql<MyParent, ClassWithNonDefaultConstructor>("SELECT A=3").First();
            Assert.AreEqual(3, a.Child.A);
        }
        #endregion

        #region Serializer
        public class CustomSerializerClass
        {
            [Column(SerializationMode = SerializationMode.Custom, Serializer = typeof(StringTrimDeserializer))]
            public readonly string Trimmed;

            public CustomSerializerClass(string trimmed)
            {
                Trimmed = trimmed;
            }
        }

        public class StringTrimDeserializer : DbObjectSerializer
        {
            public override bool CanDeserialize(Type type)
            {
                return type == typeof(String);
            }
            public override object SerializeObject(Type type, object o)
            {
                return (string)o;
            }

            public override object DeserializeObject(Type type, object o)
            {
                return ((string)o).TrimEnd();
            }
        }

        [Test]
        public void TestThatSerializerIsCopiedFromField()
        {
            var result = Connection().QuerySql<CustomSerializerClass>("SELECT Trimmed='Trim      '").First();
            Assert.AreEqual("Trim", result.Trimmed);
        }
        #endregion
    }
}
