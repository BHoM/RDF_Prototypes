
using System;
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
        private static List<Assembly> m_cachedAssemblies = null;

        [Description("Loads all the assemblies in a directory. Allows to only load BHoM assemblies and/or oM assemblies.")]
        public static List<Assembly> LoadAssembliesInDirectory(string dllDirectory = @"C:\ProgramData\BHoM\Assemblies", 
            bool onlyoMAssemblies = false, 
            bool onlyBHoMAssemblies = true, 
            bool tryLoadWithoutDependencies = false)
        {
            if (m_cachedAssemblies != null)
                return m_cachedAssemblies;

            var assemblyFiles = Directory.GetFiles(dllDirectory, "*.dll").ToList();

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

                if (assembly != null)
                    assemblies.Add(assembly);
            }

            if (onlyBHoMAssemblies)
                assemblies = assemblies.Where(assembly => Query.HasBHoMCopyright(assembly)).ToList();

            m_cachedAssemblies = assemblies;

            return assemblies;
        }

        // ------------------------------------- //

        public static List<Assembly> LoadAssembliesInDirectory(bool onlyoMAssemblies = true,
          bool onlyBHoMAssemblies = true,
          bool tryLoadWithoutDependencies = false)
        {
            return LoadAssembliesInDirectory(@"C:\ProgramData\BHoM\Assemblies", onlyoMAssemblies, onlyBHoMAssemblies, tryLoadWithoutDependencies);
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
                log.RecordError(e.Message);
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
                log.RecordError(e.Message);
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}
