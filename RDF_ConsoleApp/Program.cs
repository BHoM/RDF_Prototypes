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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BH.Engine.Adapters.RDF;
using BH.oM.Adapters.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Update;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BH.Adapters.TTL;
using Compute = BH.Engine.Adapters.RDF.Compute;
using BH.Engine.Adapters.TTL;
using BH.Adapters.Markdown;
using BH.oM.Structure.Loads;
using System.ComponentModel;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            var assemblies = Compute.LoadAssembliesInDirectory(@"C:\ProgramData\BHoM\Assemblies",
                onlyBHoMAssemblies: true, onlyoMAssemblies: true,
                searchOption: SearchOption.AllDirectories);

            Dictionary<string, Type[]> typesPerAssembly = assemblies.ToDictionary(a => a.DescriptiveName(), a => a.TryGetTypes());

            LocalRepositorySettings localRepositorySettings = new() { TryComputeURLFromFilePaths = false};
            GraphSettings graphSettings = new();

            foreach (var kv in typesPerAssembly)
            {

                string saveFolderRoot = @"C:\temp\MarkdownTests";
                string saveFolder = Path.Combine(saveFolderRoot, kv.Key);
                Directory.CreateDirectory(saveFolder);

                foreach (var classType in kv.Value)
                {
                    string filePath = Path.GetFullPath(Path.Combine(saveFolder, classType.FullNameValidChars() + ".md"));
                    string TTLClass = "";

                    // Declaration with Uri
                    string typeUri = classType.OntologyUri(graphSettings.TBoxSettings, localRepositorySettings).ToString();
                    TTLClass += $"\n\nIri: {typeUri}";
                    TTLClass += $"\n\nLabel: {classType.DescriptiveName()}";
                    TTLClass += $"\n\nType: Class";
                    TTLClass += $"\n\nDefinition: {classType.GetCustomAttribute<DescriptionAttribute>(false)?.Description}";

                    // Class Identifier
                    TTLClass += $"\n:{classType.UniqueNodeId()} rdf:type owl:Class;";

                    // Subclasses
                    List<Type> parentTypes = classType.BaseTypesNoRedundancy().Where(t => t.IsOntologyClass(graphSettings.TBoxSettings)).ToList();

                    TTLClass += "\n### Parent classes";
                    foreach (Type subClass in parentTypes)
                    {
                        TTLClass += $"\n[{subClass.DescriptiveName()}]({subClass.UniqueNodeId()}  ";
                    }

                    TTLClass += "\n\n### Object properties";
                    parentType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

                    File.WriteAllText(filePath, TTLClass);
                }

                //MarkdownAdapter mdAdapter = new MarkdownAdapter(filePath);

                //try
                //{
                //    mdAdapter.Push(kv.Value.ToList());
                //} catch (Exception ex)
                //{
                //    Console.WriteLine($"Could not write Markdown of `{kv.Key}`.");
                //}
            }
        }
    }
}
