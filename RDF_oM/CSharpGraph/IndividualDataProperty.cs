using System.Reflection;

namespace BH.oM.RDF
{
    public class IndividualDataProperty : IndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object Value { get; set; }

        // PropertyInfo that generated this Data property of this individual.
        public PropertyInfo PropertyInfo { get; set; }
    }
}
