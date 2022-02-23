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
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static void WriteWebVOWLOntology(List<string> typeFullNames, string fileName = null, string saveDirRelativeToRepoRoot = "WebVOWLOntology", HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);

            // Get the System.Types corresponding to the input typeFullNames
            List<Type> correspondingOmTypes = oMassemblies.BHoMTypes().Where(t => typeFullNames.Contains(t.AsType().FullName)).Select(ti => ti.AsType()).ToList();

            WriteWebVOWLOntology(correspondingOmTypes, fileName, saveDirRelativeToRepoRoot, exceptions, relationRecursion);
        }

        public static void WriteWebVOWLOntology(List<Type> types, string fileName = null, string saveDirRelativeToRepoRoot = "WebVOWLOntology", HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", types.Select(t => t.Name));

            // Of all the input System.Types, get also all the BHoM System.Types of all their Properties.
            //HashSet<Type> allConnectedBHoMTypes = types.AllNestedTypes();
            //types = types.Concat(allConnectedBHoMTypes).Distinct().ToList();

            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypeInfos = oMassemblies.BHoMTypes().Where(t => types.Contains(t.AsType())).ToList();

            WriteWebVOWLOntology(oMTypeInfos, fileName, saveDirRelativeToRepoRoot, exceptions, relationRecursion);
        }


        private static void WriteWebVOWLOntology(List<TypeInfo> oMTypes, string fileName, string saveDirRelativeToRepoRoot, HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = oMTypes.DictionaryGraphFromTypeInfos();
            string webVOWLJson = Engine.RDF.Convert.ToWebVOWLJson(dictionaryGraph, new HashSet<string>(oMTypes.Select(t => t.Namespace)), exceptions, relationRecursion);

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", oMTypes.Select(t => t.Name));

            if (relationRecursion > 0)
                fileName += $"-relationRecursion{relationRecursion}";

            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            webVOWLJson.WriteToJsonFile(fileName, $"..\\..\\..\\{saveDirRelativeToRepoRoot}");
        }
    }
}
