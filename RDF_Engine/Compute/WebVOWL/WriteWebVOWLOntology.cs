/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Compute
    {
        public static void WriteWebVOWLOntology(List<string> typeFullNames, LocalRepositorySettings settings, string fileName = null, HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            List<Assembly> oMassemblies = BH.Engine.Adapters.RDF.Compute.LoadAssembliesInDirectory(onlyoMAssemblies: true);

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

            List<Assembly> oMassemblies = BH.Engine.Adapters.RDF.Compute.LoadAssembliesInDirectory(onlyoMAssemblies: true);
            List<TypeInfo> oMTypeInfos = oMassemblies.BHoMTypes().Where(t => types.Contains(t.AsType())).ToList();

            WriteWebVOWLOntology(oMTypeInfos, settings, fileName, exceptions, relationRecursion);
        }


        private static void WriteWebVOWLOntology(List<TypeInfo> oMTypes, LocalRepositorySettings settings, string fileName = null, HashSet<string> exceptions = null, int relationRecursion = 0)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = oMTypes.DictionaryGraphFromTypeInfos();
            string webVOWLJson = Engine.Adapters.RDF.Convert.ToWebVOWLJson(dictionaryGraph, settings, internalNamespaces: new HashSet<string>(oMTypes.Select(t => t.Namespace)), exceptions: exceptions, relationRecursion: relationRecursion);

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
