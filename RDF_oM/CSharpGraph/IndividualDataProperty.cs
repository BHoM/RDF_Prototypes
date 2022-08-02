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

        public override bool Equals(object obj)
        {
            IndividualDataProperty o = obj as IndividualDataProperty;
            if (o == null)
                return false;

            return Individual.Equals(o.Individual) && Value.Equals(o.Value) &&
                PropertyInfo.PropertyType == o.PropertyInfo.PropertyType && PropertyInfo.DeclaringType == o.PropertyInfo.DeclaringType &&
                PropertyInfo.Name.Equals(o.PropertyInfo.Name);
        }

        public override int GetHashCode()
        {
            int A = Individual.GetHashCode();
            int B = Value.GetHashCode();
            int C = PropertyInfo.PropertyType.GetHashCode();
            int D = PropertyInfo.DeclaringType.GetHashCode();
            int E = PropertyInfo.Name.GetHashCode();
            return A + B + C + D + E;
        }
    }
}
