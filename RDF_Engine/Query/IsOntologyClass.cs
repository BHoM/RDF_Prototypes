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

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsOntologyClass(this Type type)
        {
            // This method answer this question:
            // do we want this type to appear as a class in the ontology?

            if (type.IsBHoMType())
                return true; // yes, if it is a BHoM Type

            // We need to work out what we want to do for everything else, including collections of BHoM Types (e.g. List<Beam>).
            // For now, we assume that everything else will be stored as a serialised Json in the ontology.
            return false;
        }
    }
}
