using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static SortedDictionary<string, string> WebVOWLJsonPerNamespace(List<TypeInfo> oMTypes, TBoxSettings settings, List<string> namespaceToConsider = null, List<string> typeNamesToConsider = null, int namespaceGroupDepth = 3)
        {
            var oMTypesPerNamespace = Query.OMTypesPerNamespace(oMTypes, namespaceToConsider, typeNamesToConsider, namespaceGroupDepth);

            var res = WebVOWLJsonPerNamespace(oMTypesPerNamespace, settings);

            return res;
        }

        public static SortedDictionary<string, string> WebVOWLJsonPerNamespace(IDictionary<string, List<TypeInfo>> oMTypesGroupsPerNamespace, TBoxSettings settings)
        {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>(new NaturalSortComparer<string>());

            foreach (var kv in oMTypesGroupsPerNamespace)
            {
                // Extract a dictionary representation of the BHoM Ontology Graph
                Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = kv.Value.DictionaryGraphFromTypeInfos();
                string webVOWLJson = Engine.RDF.Convert.ToWebVOWLJson(dictionaryGraph, settings, new HashSet<string> { kv.Key });

                result[kv.Key] = webVOWLJson;
            }

            return result;
        }
    }
}