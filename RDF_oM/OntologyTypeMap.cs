using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public static class OntologyDataTypeMap
    {
        public static Dictionary<Type, string> ToOntologyDataType = new Dictionary<Type, string>()
        {
            { typeof(string), "xsd:string" },
            { typeof(bool), "xsd:boolean" },
            { typeof(int), "xsd:integer" },
            { typeof(double), "xsd:double" },
            { typeof(float), "xsd:float" },
            { typeof(decimal), "xsd:decimal" },
            { typeof(Guid), "xsd:string"}
        };

        public static Dictionary<string, Type> FromOntologyDataType = new Dictionary<string, Type>()
        {
            { "string",typeof(string) },
            { "boolean", typeof(bool)},
            { "integer" , typeof(int)},
            { "double", typeof(double) },
            { "float" , typeof(float)},
            { "decimal", typeof(decimal)}
        };
    }
}
