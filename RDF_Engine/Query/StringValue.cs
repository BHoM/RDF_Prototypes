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
        public static string StringValue(this IndividualDataProperty idp)
        {
            Type individualObjectType = idp.Value.GetType();
            if (OntologyTypeMap.ToOntologyDataType.ContainsKey(individualObjectType))
                return idp.Value.ToString(); // we can just return the ToString()

            // We must use our fallback for unknown conversions, serializing to Json.
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string serializedValue = JsonConvert.SerializeObject(idp.Value, settings);

            // Encode to base64 to avoid escaping quote problems
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(serializedValue);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
