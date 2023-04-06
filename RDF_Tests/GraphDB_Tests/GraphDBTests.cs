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
        //[Test] // Restore when fixed as below.
        public void PullFromRepoTest()
        {
            // This test should be corrected to first verify if the connectivity to GraphDB is available.
            // If not available, the test should not run, to avoid false failures. Commenting out the code below.

            var result = BH.Engine.Adapters.GraphDB.Compute.PullFromRepo(repositoryName:"myrepo123", run:true);
            result.ShouldNotBeNull();
        }
    }
}
