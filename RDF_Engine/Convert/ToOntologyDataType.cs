using BH.oM.Base;
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
    public static partial class Convert
    {
        public static string ToOntologyDataType(this Type t)
        {
            string ontologyDataType = null;
            OntologyDataTypeMap.ToOntologyDataType.TryGetValue(t, out ontologyDataType);

            if (ontologyDataType != null)
                return ontologyDataType;

            // Fallback
            return typeof(Base64JsonSerialized).UniqueNodeId(); // assumes that the Data Type is already added to the Graph under this Identifier.
        }
    }
}
