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
    public class CSharpGraph : IObject
    {
        // Types that will correspond to ontology Classes (T Box)
        public HashSet<Type> Classes { get; set; } = new HashSet<Type>();

        // Relations between Classes (TBox)
        public HashSet<ObjectProperty> ObjectProperties { get; set; } = new HashSet<ObjectProperty>();

        public HashSet<DataProperty> DataProperties { get; set; } = new HashSet<DataProperty>();

        // All individuals (A Box)
        public HashSet<object> AllIndividuals { get; set; } = new HashSet<object>();

        // Relations between individuals (A Box)
        public HashSet<IndividualRelation> IndividualRelations { get; set; } = new HashSet<IndividualRelation>();

        // Settings used to compose this Graph ontology.
        public OntologySettings OntologySettings { get; set; }
    }

    public abstract class IClassRelation : IObject
    {
        // CSharp PropertyInfos can be seen as the correspondant to Ontology Object Properties.
        public PropertyInfo PropertyInfo { get; set; }

        public override bool Equals(object obj)
        {
            IClassRelation clRel = obj as IClassRelation;
            if (clRel == null || this.GetType() != obj.GetType())
                return false;

            return FullNameValidChars(PropertyInfo) == FullNameValidChars(clRel.PropertyInfo);
        }

        public override int GetHashCode()
        {
            return FullNameValidChars(PropertyInfo).GetHashCode();
        }

        private static string FullNameValidChars(PropertyInfo pi)
        {
            return $"{pi.DeclaringType.FullName}.{pi.Name}";
        }
    }

    public class ObjectProperty : IClassRelation // aka "HasProperty" when the range is another class in the Ontology.
    {
        public Type DomainClass { get; set; }

        public Type RangeClass { get; set; }
    }

    public class DataProperty : IClassRelation // aka "HasProperty" when the range is NOT another class in the Ontology.
    {
        public Type DomainClass { get; set; }

        public Type RangeType { get; set; } // In a DataProperty, the range will NOT correspond to an Ontology Class.
    }

    public interface IndividualRelation : IObject
    {
        object Individual { get; set; }
    }

    public class IndividualObjectProperty : IndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object RangeIndividual { get; set; }

        // Class relation corresponding to these Individuals' relation.
        public ObjectProperty HasProperty { get; set; }
    }

    public class IndividualDataProperty : IndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object Value { get; set; }

        // PropertyInfo that generated this Data property of this individual.
        public PropertyInfo PropertyInfo { get; set; }
    }


}
