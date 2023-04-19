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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Compute
    {
        private static List<Assembly> m_cachedAssemblies = null;

        [Description("Loads all the assemblies in a directory. Allows to only load BHoM assemblies and/or oM assemblies.")]
        public static IEnumerable<Assembly> LoadAssembliesInDirectory(string dllDirectory = @"C:\ProgramData\BHoM\Assemblies",
            bool onlyoMAssemblies = false,
            bool onlyBHoMAssemblies = true,
            bool tryLoadWithoutDependencies = false,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {

            return loadAssembliesInDirectory(dllDirectory, onlyoMAssemblies, onlyBHoMAssemblies, tryLoadWithoutDependencies, searchOption).Memoize();
        }

        [Description("Loads all the assemblies in a directory. Allows to only load BHoM assemblies and/or oM assemblies.")]
        public static IEnumerable<Assembly> loadAssembliesInDirectory(string dllDirectory = @"C:\ProgramData\BHoM\Assemblies",
        bool onlyoMAssemblies = false,
        bool onlyBHoMAssemblies = true,
        bool tryLoadWithoutDependencies = false,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var assemblyFiles = Directory.GetFiles(dllDirectory, "*.dll", searchOption).ToList();

            if (onlyoMAssemblies)
                assemblyFiles = assemblyFiles.Where(name => name.EndsWith("_oM.dll") || name.EndsWith("BHoM.dll")).ToList();

            List<Assembly> assemblies = new List<Assembly>();

            foreach (var assemblyFile in assemblyFiles)
            {
                Assembly assembly = null;

                if (tryLoadWithoutDependencies)
                    TryLoadAssemblyFileWithoutDependencies(assemblyFile, out assembly);

                if (assembly == null)
                    TryLoadAssemblyFile(assemblyFile, out assembly);

                if (assembly == null && !tryLoadWithoutDependencies)
                    TryLoadAssemblyFileWithoutDependencies(assemblyFile, out assembly); // as last resort if it wasn't tried before.

                if (assembly == null)
                    continue;

                if (onlyBHoMAssemblies && !Query.HasBHoMCopyright(assembly))
                    continue;

                m_cachedAssemblies.Add(assembly);

                yield return assembly;
            }
        }

        // ------------------------------------- //

        private static bool TryLoadAssemblyFile(string assemblyFile, out Assembly assembly)
        {
            Console.Write($"\nLoading {assemblyFile} with dependencies (LoadFrom): ");

            assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(assemblyFile); // using LoadFrom() instead of Load() makes it work better for some reason! Especially when running from VS instead of Linqpad. https://stackoverflow.com/a/20607325/3873799

                Console.Write($"Done\n");

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log.RecordError(e.Message);
            }

            return false;
        }

        private static bool TryLoadAssemblyFileWithoutDependencies(string assemblyFile, out Assembly assembly)
        {
            Console.Write($"\nTrying to load {assemblyFile} without dependencies (ReflectionOnlyLoad): ");

            assembly = null;
            try
            {
                assembly = Assembly.ReflectionOnlyLoad(assemblyFile); // this prevents problems with e.g. Revit where certain dependencies cannot be fully loaded (as if for execution).

                Console.Write($"Done\n");

                return true;

            }
            catch (Exception e)
            {
                Log.RecordError(e.Message);
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}
