using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public class CSharpOntology : IObject
    {
        // Used to create the Classes (T Box)
        public HashSet<Type> Classes { get; set; } = new HashSet<Type>();

        // Used to create the Individuals (A Box)
        public HashSet<IndividualRelation> IndividualRelations { get; set; } = new HashSet<IndividualRelation>();

        // Used to create the TBox
        public HashSet<PropertyInfo> ClassRelations { get; set; } = new HashSet<PropertyInfo>(); // both ObjectProperties and DataProperties. Which one can be derived by PropertyInfo.PropertyType (the range type)

        // Might consider redundant
        public HashSet<object> AllIndividuals { get; set; } = new HashSet<object>();
    }

    public class IndividualRelation : IObject
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.

        public PropertyInfo PropertyInfo { get; set; }
        public object DomainIndividual { get; set; } = null;
        public object RangeIndividual { get; set; } = null;
    }
}
