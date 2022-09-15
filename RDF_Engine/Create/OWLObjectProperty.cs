using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an instance of the given type, by using a specified contructor chosen via its index. 0 is the first ctor, 1 the second, and so on. Ctor input args can be specified.")]
        public static List<OWLObjectProperty> OWLObjectPropertyType(object domainClass, string objectPropertyName, OWLObjectPropertyType OWLObjectPropertyType)
        {

            return new List<OWLObjectProperty>();
        }
    }
}