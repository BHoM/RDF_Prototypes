﻿using BH.oM.Analytical.Results;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;
using BH.oM.RDF;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    class Program
    {
        public static void Main(string[] args = null)
        {
            //List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);

            // HashSet<TypeInfo> oMTypes = new HashSet<TypeInfo>(oMassemblies.SelectMany(a => a.DefinedTypes));

            // Invoke all static methods in `Tests` class
            typeof(Tests).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));
        }
    }
}
