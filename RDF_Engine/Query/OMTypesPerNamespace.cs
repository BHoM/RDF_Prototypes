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
    public static partial class Query
    {
        public static SortedDictionary<string, List<TypeInfo>> OMTypesPerNamespace(List<TypeInfo> oMTypes, List<string> namespaceToConsider = null, List<string> typeNamesToConsider = null, int namespaceGroupDepth = 3)
        {
            // Null check
            oMTypes = oMTypes.Where(t => t != null && t.Namespace != null).ToList();

            // Filters
            if (namespaceToConsider != null && namespaceToConsider.All(f => !string.IsNullOrWhiteSpace(f)))
                oMTypes = oMTypes.Where(t => namespaceToConsider.Any(nsf =>
                {
                    string typeFullName = t.FullNameValidChars();
                    if (nsf.StartsWith("BH."))
                        return typeFullName.StartsWith(nsf);
                    else
                        return typeFullName.Contains(nsf);
                })).ToList();


            if (typeNamesToConsider != null && typeNamesToConsider.All(f => !string.IsNullOrWhiteSpace(f)))
                oMTypes = oMTypes.Where(t => typeNamesToConsider.Any(tn =>
                {
                    if (tn.StartsWith("BH."))
                        return t.FullNameValidChars() == tn;
                    else
                        return t.NameValidChars().Contains(tn);
                })).ToList();


            Dictionary<string, List<TypeInfo>> oMTypesPerNamespace = oMTypes.GroupBy(t => t.NamespaceGroup()).ToDictionary(g => g.Key, g => g.ToList());

            return new SortedDictionary<string, List<TypeInfo>>(oMTypesPerNamespace, new NaturalSortComparer<string>());
        }
    }
}