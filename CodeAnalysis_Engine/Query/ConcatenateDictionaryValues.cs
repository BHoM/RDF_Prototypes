using BH.oM.Base.Attributes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.External.RDF
{
    public static partial class Query
    {
        [Description("Concatenate two dictionaries whose Value is a List. The values will be added to the first dictionary specified." +
            "If a duplicate key is found, elements from the second dictionary are appended to the first dictionary's list. ")]
        [Input("dict1", "First dictionary. Key-value pairs from the second dictionary will be added to this dictionary.")]
        [Input("dict2", "Second dictionary. Key-value pairs from the this dictionary will be added to the first dictionary. If a duplicate key is found, the items from the list are concatenated.")]
        [Input("distinct", "If true, removes duplicate items from all lists.")]
        public static void ConcatenateDictionaryValues<K, L>(this Dictionary<K, List<L>> dict1, Dictionary<K, List<L>> dict2, bool distinct = false)
        {
            foreach (var kv2 in dict2)
            {
                List<L> v1 = null;
                if (dict1.TryGetValue(kv2.Key, out v1))
                {
                    if (v1 != null)
                        dict1[kv2.Key].AddRange(kv2.Value);
                }
                else
                    dict1[kv2.Key] = kv2.Value;
            }

            if (distinct)
            {
                Dictionary<K, List<L>> distinctResult = new Dictionary<K, List<L>>();
                foreach (var kv1 in dict1)
                {
                    distinctResult[kv1.Key] = kv1.Value.Distinct().ToList();
                }

                dict1 = distinctResult;
            }
        }
    }
}
