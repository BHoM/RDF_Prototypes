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
            // A CSharp PropertyInfo can corresponds to an Object Property if:
            //   1) The domain of the relation (the PropertyInfo's DeclaringType) can be translated to an Ontology class;
            //   2) The range of the relation (the PropertyInfo's PropertyType):
            //       2a) can be translated to an Ontology class, OR
            //       2b) is a List of objects, whether it contains objects translatable to Ontology class or DataTypes (https://github.com/BHoM/RDF_Prototypes/issues/17)

            return pi.PropertyType.IsOntologyClass() && pi.DeclaringType.IsOntologyClass() || pi.PropertyType.IsList();
        }
    }
}
