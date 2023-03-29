using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Adapters.RDF;
using NUnit.Framework;
using Shouldly;

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
    }
}
