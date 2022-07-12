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
            if (t == null)
                return false;

            return typeof(IList).IsAssignableFrom(t);
        }

        public static bool IsListOfOntologyClasses(this Type sourceType, object sourceObj)
        {
            // Make sure the type is a List.
            if (!sourceType.IsList())
                return false;

            // Check the List generic argument.
            Type[] genericArgs = sourceType.GetGenericArguments();

            if (genericArgs.Length != 1)
                return false;

            // If the List generic arg can be translated to an Ontology class, job done.
            if (genericArgs.First() != typeof(System.Object))
                return genericArgs.First().IsOntologyClass();

            // If the List generic arg is System.Object, the objects may still be Ontology classes that have been boxed.
            if (sourceObj != null && genericArgs.First() == typeof(System.Object))
            {
                List<object> objList = sourceObj as List<object>;

                // Unbox the objects and see if their actual type is an Ontology class.
                return objList.All(o => o.GetType().IsOntologyClass());
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
