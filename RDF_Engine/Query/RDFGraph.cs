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
using BH.Engine.Reflection;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Graph DotNetGraph(this IObject iobj, LocalRepositorySettings settings = null)
        {
            if (iobj == null)
                return null;

            Graph g = new Graph();
            Type t = iobj.GetType();

            if (settings == null)
                settings = new LocalRepositorySettings();

            // Create node for this type
            g.CreateUriNode(t.GithubURI(settings));

            AddToGraph(g, iobj, t, settings);


            return g;
        }

        private static void AddToGraph(Graph g, object obj, Type type, LocalRepositorySettings settings)
        {
            // To continue, the type must either be a BHoM Type or "a collection of BHoM Types".
            if (!(type.IsBHoMType() || type.IsCollectionOfBHoMTypes()))
                return;

            if (obj.IsNullOrEmpty())
                return;

            // Create node for this type
            //if (isCollectionOfBHoMTypes)

            IUriNode node = g.CreateUriNode(type.GithubURI(settings));


            var properties = type.GetProperties();
            foreach (var propInfo in properties)
            {
                Type propType = propInfo.PropertyType;
                if (!(propType.IsBHoMType() || propType.IsCollectionOfBHoMTypes()))
                    return;

                object propValue = propInfo.GetValue(obj);
                if (propValue.IsNullOrEmpty())
                    continue;


                Uri propUri = propInfo.GithubURI(settings);

                IUriNode hasPropNode = g.CreateUriNode(propUri);

                //g.Assert(new Triple(node, hasPropNode, propUri));

            }
        }


        private static void AddNodeToGraph()
        {

        }


        private static void AddCollectionToGraph()
        {

        }
    }
}
