using System;

namespace BH.oM.RDF
{
    public class IndividualObjectProperty : IndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object RangeIndividual { get; set; }

        // Class relation corresponding to these Individuals' relation.
        public ObjectProperty HasProperty { get; set; }

        public override bool Equals(object obj)
        {
            IndividualObjectProperty o = obj as IndividualObjectProperty;
            if (o == null)
                return false;

            return Individual == o.Individual && RangeIndividual == o.RangeIndividual &&
                HasProperty.DomainClass == o.HasProperty.DomainClass && HasProperty.RangeClass == o.HasProperty.RangeClass;
        }

        public override int GetHashCode()
        {
            int A = Individual.GetHashCode();
            int B = RangeIndividual.GetHashCode();
            int C = HasProperty.DomainClass.GetHashCode();
            int D = HasProperty.RangeClass.GetHashCode();
            return A + B + C + D;
        }
    }
}
