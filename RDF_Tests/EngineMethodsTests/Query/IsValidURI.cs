using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BH.Engine.Adapters.RDF;
using Shouldly;

namespace RDF_Tests.EngineMethodsTests
{
    internal class IsValidURI : BH.Test.RDF.Test
    {
        [Test]
        public static void ValidURITest()
        {
            Query.IsValidURI("http://Bhom.xyz").ShouldBeTrue();
        }

        [Test]
        public static void InvalidURITest()
        {
            Assert.Throws<ArgumentException>(() => Query.IsValidURI("www.Bhom.xyz"));
        }

    }
}
