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
        public static bool IsList(this Type t)
        {
            return typeof(IList).IsAssignableFrom(t);
        }

        public static bool IsListOfOntologyClasses(this Type sourceType, object sourceObj)
        {
            if (!sourceType.IsList())
                return false;

            Type[] genericArgs = sourceType.GetGenericArguments();

            if (genericArgs.Length != 1)
                return false;

            if (genericArgs.First() != typeof(System.Object))
            {
                return genericArgs.First().IsOntologyClass();
            }

            if (genericArgs.First() == typeof(System.Object))
            {
                // Let's see if the individual objects of the list have a common type.
                List<object> objList = sourceObj as List<object>;

                // Check if all objects have common parent types between each other.
                HashSet<Type> commonParentTypes = objList.ListElementsCommonParentTypes();

                // Check if at least one of the common parent types is an ontology class. If so, the list can be considered a list of ontology classes.
                bool isAnyCommonParentTypeAnOntologyClass = commonParentTypes.Any(t => t.IsOntologyClass());

                return isAnyCommonParentTypeAnOntologyClass;
            }

            return false;
        }

        public static bool IsListOfOntologyClasses(this IndividualObjectProperty iop)
        {
            Type rangeType = iop.RangeIndividual.GetType();

            return IsListOfOntologyClasses(rangeType, iop.RangeIndividual);
        }

        public static bool IsListOfDatatypes(this Type t)
        {
            if (t.IsList())
            {
                Type[] genericArgs = t.GetGenericArguments();

                if (genericArgs.Length == 1 && genericArgs.First().IsDataType())
                    return true;
            }

            return false;
        }
    }
}
