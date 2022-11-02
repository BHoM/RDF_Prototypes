using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
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
        public static string DataPropertyStringValue(this IndividualDataProperty idp)
        {
            return DataPropertyStringValue(idp.Value);
        }

        public static string DataPropertyStringValue(object idpValue)
        {
            Type individualObjectType = idpValue.GetType();
            if (OntologyDataTypeMap.ToOntologyDataType.ContainsKey(individualObjectType))
                return idpValue.ToString(); // we can just return the ToString()

            // We must use our fallback for unknown conversions, serializing to Json.
            return idpValue.ToBase64JsonSerialized();
        }
    }
}
