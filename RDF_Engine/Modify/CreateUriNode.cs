using BH.oM.Base;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;


namespace BH.Engine.RDF
{
    public static partial class Modify
    {
        [Description("Modifies the Given RDF IGraph adding to it a URI Node, which is obtained by inferring the ")]
        public static IUriNode CreateUriNode(this IGraph graph, Type t, LocalRepositorySettings settings)
        {
            if (!t.FullName.StartsWith("BH.oM"))
            {
                Log.RecordError("This method only supports BHoM types.");
                return null;
            }    

            return graph.CreateUriNode(t.GithubURI(settings)); // UriFactory.Create(t.UriFromType()
        }
    }
}
