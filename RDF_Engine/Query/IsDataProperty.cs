using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsDataProperty(this PropertyInfo pi)
        {
            // An ontology Data property is a relation from a class of an ontology to a Data Type.
            // A CSharp PropertyInfo can corresponds to an Object Property 
            // if the range of the relation (= the property type) is NOT an ontology class,
            // while the domain of the relation (the PropertyInfo's Declaring Type) is an ontology class.

            return pi.PropertyType.IsDataType() && pi.DeclaringType.IsOntologyClass();
        }
    }
}
