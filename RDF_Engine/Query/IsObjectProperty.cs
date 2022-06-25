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
        public static bool IsObjectProperty(this PropertyInfo pi)
        {
            // An ontology Object property is a relation between two classes of an ontology.
            // A CSharp PropertyInfo can corresponds to an Object Property 
            // if the range of the relation (= the property type) is an ontology class,
            // and so is the domain of the relation (the PropertyInfo's Declaring Type).

            // Lists are also to be considered ObjectProperties: https://github.com/BHoM/RDF_Prototypes/issues/17
            return pi.PropertyType.IsOntologyClass() && pi.DeclaringType.IsOntologyClass() || pi.PropertyType.IsList();
        }
    }
}
