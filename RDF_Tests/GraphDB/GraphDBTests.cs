using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Adapters.RDF;
using BH.oM.Adapters.RDF;
using BH.oM.Architecture.Elements;
using NUnit.Framework;
using Shouldly;
using BH.Engine.Adapters.TTL;
using Compute = BH.Engine.Adapters.RDF.Compute;
using VDS.RDF.Query;
using VDS.RDF;
using VDS.RDF.Writing;

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

        [Test]
        public void PullFromRepoReadBack() 
        {
            string userInput = "PREFIX bh: <https://bhom.xyz/ontology/> SELECT ?predicate ?object WHERE {bh:BH.oM.Architecture.Elements.Room ?predicate ?object.}";
            var results = BH.Engine.Adapters.GraphDB.Compute.PullFromRepo(userInput, repositoryName: "TestRepository", run: true);
            results.ShouldNotBeNull();

            Room room = new Room();
            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object>() { room });
            string TTLGraph = cSharpGraph.ToTTL();

            IGraph graph = new Graph();
            INode subject = graph.CreateUriNode(new Uri("https://bhom.xyz/ontology/BH.oM.Architecture.Elements.Room"));

            foreach (SparqlResult result in results)
            {
                INode predicate = result["predicate"].CopyNode(graph);
                INode obj = result["object"].CopyNode(graph);

                graph.Assert(new Triple(subject, predicate, obj));
            }

            CompressingTurtleWriter turtleWriter = new CompressingTurtleWriter();
            string turtleString;
            using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
            {
                turtleWriter.Save(graph, stringWriter);
                turtleString = stringWriter.ToString();
            }

            var res = BH.Engine.Adapters.TTL.Convert.FromTTL(turtleString);

        }

        //[Test] 
        // Test requires GraphDB to be running
        public void PushToGraphDB()
        {
            Room room = new Room();

            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string TTLfilepath = Path.Combine(userDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_GraphDBPush.ttl");

            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object>() { room });
            string TTLGraph = cSharpGraph.ToTTL(filepath : TTLfilepath);
            string testRepositoryName = "TestRepository";

            bool httpResponse = BH.Engine.Adapters.GraphDB.Compute.PostToRepo(TTLfilepath, repositoryName: testRepositoryName, run: true);

            Assert.IsTrue(httpResponse);
        }
    }
}
