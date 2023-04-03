using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Adapters.RDF;
using NUnit.Framework;
using Shouldly;
using VDS.RDF.Query.Algebra;

namespace RDF_Tests.GraphDB
{
    internal class GraphDBTests
    {
        [Test]
        public void PullFromRepoTest()
        {
            var result = BH.Engine.Adapters.GraphDB.Compute.PullFromRepo(repositoryName:"myrepo123", run:true);
            result.ShouldNotBeNull();
        }

        [Test]
        public void PullFromRepoUserInput()
        {
            string userInput = "select ?subject ?predicate ?object WHERE {?subject ?predicate ?object.}";
            var result = BH.Engine.Adapters.GraphDB.Compute.PullFromRepo(userInput,repositoryName: "myrepo123", run: true);
            result.ShouldNotBeNull();
        }
    }
}
