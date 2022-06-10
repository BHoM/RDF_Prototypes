using BH.oM.Analytical.Results;
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
using System.ComponentModel;

namespace BH.Test.RDF
{
    public static partial class Assert
    {
        public static void IsTrue(bool value, string error = null)
        {
            if (!value)
                throw new Exception(error ?? "Test failed.");
        }

        public static void TotalCount(int value, int targetValue, string variableName)
        {
            if (value != targetValue)
                throw new Exception($"Test failed: total {variableName} count should have been {targetValue} but was {value}.");
        }
    }
}
