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
        public static bool IsCollectionOfOntologyDataTypes(this Type type)
        {
            // We need to work out what we want to do for everything else, including collections of BHoM Types (e.g. List<Beam>).
            // For now, we assume that everything else will be translated to a Data property ("SerialisedJson").

            // This only considers IEnums with one generic argument. Dictionaries are not covered.
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            if (!type.InnermostType().IsOntologyClass())
                return true;

            return false;
        }
    }
}
