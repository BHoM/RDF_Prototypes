using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static void WriteWebVOWLOntology(List<string> typeFullNames, LocalRepositorySettings settings, string fileName = null, HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);

            // Get the System.Types corresponding to the input typeFullNames
            List<Type> correspondingOmTypes = oMassemblies.BHoMTypes().Where(t => typeFullNames.Contains(t.AsType().FullName)).Select(ti => ti.AsType()).ToList();

            WriteWebVOWLOntology(correspondingOmTypes, settings, fileName, exceptions, relationRecursion);
        }

        public static void WriteWebVOWLOntology(List<Type> types, LocalRepositorySettings settings, string fileName = null,  HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", types.Select(t => t.Name));

            // Of all the input System.Types, get also all the BHoM System.Types of all their Properties.
            //HashSet<Type> allConnectedBHoMTypes = types.AllNestedTypes();
            //types = types.Concat(allConnectedBHoMTypes).Distinct().ToList();

            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypeInfos = oMassemblies.BHoMTypes().Where(t => types.Contains(t.AsType())).ToList();

            WriteWebVOWLOntology(oMTypeInfos, settings, fileName, exceptions, relationRecursion);
        }


        private static void WriteWebVOWLOntology(List<TypeInfo> oMTypes, LocalRepositorySettings settings, string fileName = null, HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = oMTypes.DictionaryGraphFromTypeInfos();
            string webVOWLJson = Engine.RDF.Convert.ToWebVOWLJson(dictionaryGraph, settings, internalNamespaces: new HashSet<string>(oMTypes.Select(t => t.Namespace)), exceptions: exceptions, relationRecursion: relationRecursion);

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", oMTypes.Select(t => t.Name));

            if (relationRecursion > 0)
                fileName += $"-relationRecursion{relationRecursion}";

            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            webVOWLJson.WriteToJsonFile(fileName, $"..\\..\\..\\{settings.SaveDir_RelativeToRoot}");
        }
    }
}
