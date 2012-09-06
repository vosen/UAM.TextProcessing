using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Vosen.SQLFilter;
using Antlr.Runtime.Tree;
using System.Linq.Expressions;

namespace Sonar.Tests.SQLFilter
{
    [TestFixture]
    public class Compiler
    {
        [Test]
        public void SimpleStringExpr()
        {
            // Setup
            string input = "\'%%ho sama\' LIKE Map";
            var filter = new Filter<FakeServer>(input);
            // Go
            var s1 = new FakeServer(){ Map = "ho sama" };
            var s2 = new FakeServer() { Map = "asdho sama" };
            var s3 = new FakeServer() { Map = "asdHo samA" };
            var s4 = new FakeServer() { Map = "Ho samĄ" };
             
            Assert.AreEqual(true, filter.Function(s1));
            Assert.AreEqual(true, filter.Function(s2));
            Assert.AreEqual(true, filter.Function(s3));
            Assert.AreEqual(false, filter.Function(s4));
        }

        [Test]
        public void EmptyStringExpr()
        {
            // Setup
            string input = "\'\' LIKE Map";
            var filter = new Filter<FakeServer>(input);
            var s1 = new FakeServer() { Map = "" };
            var s2 = new FakeServer() { Map = "ą" };
            Assert.AreEqual(true, filter.Function(s1));
            Assert.AreEqual(false, filter.Function(s2));
        }

        [Test]
        public void InvalidPropertyExpr()
        {
            // Setup
            string input = "\'tr-_ala%lala\' LIKE Kabanos";
            var filter = new Filter<FakeServer>(input);
            var s1 = new FakeServer() { Map = "" };
            var s2 = new FakeServer() { Map = "ą" };
            Assert.AreEqual(true, filter.Function(s1));
            Assert.AreEqual(true, filter.Function(s2));
        }

        [Test]
        public void InvalidPropertyGroupExpr()
        {
            // Setup
            string input = "(\'tr-_ala%lala\' LIKE Kabanos) OR (Map Like \'a\')";
            var filter = new Filter<FakeServer>(input);
            var s1 = new FakeServer() { Map = "" };
            var s2 = new FakeServer() { Map = "ą" };
            var s3 = new FakeServer() { Map = "a" };
            Assert.AreEqual(false, filter.Function(s1));
            Assert.AreEqual(false, filter.Function(s2));
            Assert.AreEqual(true, filter.Function(s3));
        }

        [Test]
        public void StringExprComplexPattern()
        {
            // Setup
            string input = @"(Map LIKE '\%\^%\_')";
            var filter = new Filter<FakeServer>(input);
            var s1 = new FakeServer() { Map = "kabanosnos" };
            var s2 = new FakeServer() { Map = @"%^_" };
            var s3 = new FakeServer() { Map = "a" };
            Assert.AreEqual(false, filter.Function(s1));
            Assert.AreEqual(true, filter.Function(s2));
            Assert.AreEqual(false, filter.Function(s3));
        }

        [Test]
        public void BoolExpr()
        {
            // Setup
            string input = "TRUE IS IsLong";
            var filter = new Filter<FakeServer>(input);
            // Go
            var s1 = new FakeServer() { IsLong = false };
            var s2 = new FakeServer() { IsLong = true };
            Assert.AreEqual(false, filter.Function(s1));
            Assert.AreEqual(true, filter.Function(s2));
        }


        [Test]
        // This test might be surprising but it works as desgined (tm). 
        // Integer values are clipped and casted to the type of property.
        // So in the compiled filter Players is compared to Int32.MaxValue.
        // It could be done smarter with compiling out this whole num expression, but i don't care.
        public void ExprGroup()
        {
            // Setup
            string input = @"((TRUE IS IsLong) AND (IsSecure IS TRUE)) OR (Map LIKE '\%\^%\_') OR Players = 00791234139281892372871489237123";
            var filter = new Filter<FakeServer>(input);
            // Go
            var s1 = new FakeServer() { IsLong = null, IsSecure = null, Map = null, Players= Int32.MaxValue };
            Assert.AreEqual(true, filter.Function(s1));
        }

        [Test]
        public void TristateBool()
        {
            // Setup
            string input = "(IsLong IS TRUE) OR (IsSecure IS TRUE)";
            var filter = new Filter<FakeServer>(input);
            // Go
            var s1 = new FakeServer() { IsLong = null, IsSecure = true };
            Assert.IsTrue(filter.Function(s1));
        }

        [Test]
        public void NumExprNumberClamping()
        {
            string input = @"Players = -00791234139281892372871489237123";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer(){ Players = Int32.MinValue };
            Assert.IsTrue(filter.Function(s1));
        }

        [Test]
        public void NonNullableNum()
        {
            string input = @"78123 <= ID";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer() { ID = 99999 };
            var s2 = new FakeServer() { ID = 78123 };
            var s3 = new FakeServer() { ID = 0 };
            Assert.IsTrue(filter.Function(s1));
            Assert.IsTrue(filter.Function(s2));
            Assert.IsFalse(filter.Function(s3));
        }

        [Test]
        public void NonNullableBool()
        {
            string input = @"IsCached IS TRUE";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer() { IsCached = true};
            var s2 = new FakeServer() { IsCached = false };
            Assert.IsTrue(filter.Function(s1));
            Assert.IsFalse(filter.Function(s2));
        }

        [Test]
        public void EnumExpr()
        {
            string input = @"Subtype = 'Listen'";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer() { Subtype = ServerSubtype.Listen };
            var s2 = new FakeServer() { Subtype = ServerSubtype.TV };
            Assert.IsTrue(filter.Function(s1));
            Assert.IsFalse(filter.Function(s2));
        }

        [Test]
        public void NegatedAnd()
        {
            string input = @"NOT (Subtype = 'Listen' AND  Map Like '%ąęw%')";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer() { Subtype = ServerSubtype.Listen };
            var s2 = new FakeServer() { Subtype = ServerSubtype.TV, Map = "ąęw" };
            var s3 = new FakeServer() { Subtype = ServerSubtype.Listen, Map = "stąęw" };
            Assert.IsTrue(filter.Function(s1));
            Assert.IsTrue(filter.Function(s2));
            Assert.IsFalse(filter.Function(s3));
        }

        [Test]      
        public void NegatedOr()
        {
            string input = @"NOT (Subtype = 'Listen' OR Map LIKE 'sKazA')";
            var filter = new Filter<FakeServer>(input);
            /* now check our tree */
            var s1 = new FakeServer() { Subtype = ServerSubtype.Listen, Map = "skaza" };
            var s2 = new FakeServer() { Subtype = ServerSubtype.TV };
            Assert.IsFalse(filter.Function(s1));
            Assert.IsTrue(filter.Function(s2));
        }

        [Test]
        public void IPExpr()
        {
            // Setup
            string input = "FakeAddress < 87.87.87.87";
            var filter = new Filter<FakeServer>(input);
            // Go
            var s1 = new FakeServer() { FakeAddress = System.Net.IPAddress.Parse("87.87.87.87") };
            var s2 = new FakeServer() { FakeAddress = System.Net.IPAddress.Parse("86.87.87.87") };
            Assert.IsFalse(filter.Function(s1));
            Assert.IsTrue(filter.Function(s2));
        }
    }
}
