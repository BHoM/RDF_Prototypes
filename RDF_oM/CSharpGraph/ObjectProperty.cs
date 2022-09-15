using BH.oM.Base;
using System;
using System.ComponentModel;

namespace BH.oM.RDF
{
    public interface IObjectProperty : IClassRelation
    {
        Type DomainClass { get; set; }

        Type RangeClass { get; set; }
    }

    [Description("Identifies a relation between two Types in a CSharp graph that is akin to an Object Property relation in an Ontology format." +
        "If the Range class is set to a Type that is another class in the Ontology, the ObjectProperty relation can be seen as a 'HasProperty' relation.")]
    public class ObjectProperty : ClassRelation, IObjectProperty // aka "HasProperty" when the range is another class in the Ontology.
    {
        public Type DomainClass { get; set; }

        public Type RangeClass { get; set; }
    }

    [Description("Identifies a relation between two Types in a CSharp graph that is akin to an Object Property relation in an Ontology format." +
    "If the Range class is set to a Type that is another class in the Ontology, the ObjectProperty relation can be seen as a 'HasProperty' relation.")]
    public class OWLObjectProperty : ObjectProperty // aka "HasProperty" when the range is another class in the Ontology.
    {
        public OWLObjectPropertyType OWLObjectPropertyType { get; set; } = OWLObjectPropertyType.Undefined;
    }
}
