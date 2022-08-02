using System;
using System.ComponentModel;

namespace BH.oM.RDF
{
    [Description("Identifies a relation between two Types in a CSharp graph that is akin to an Object Property relation in an Ontology format." +
        "If the Range class is set to a Type that is another class in the Ontology, the ObjectProperty relation can be seen as a 'HasProperty' relation.")]
    public class ObjectProperty : IClassRelation // aka "HasProperty" when the range is another class in the Ontology.
    {
        public Type DomainClass { get; set; }

        public Type RangeClass { get; set; }
    }
}
