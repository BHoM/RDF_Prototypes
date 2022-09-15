using System;
using System.ComponentModel;

namespace BH.oM.RDF
{
    [Description("Identifies a relation between two Types in a CSharp graph that is akin to a Data Property relation in an Ontology format." +
    "The RangeType should be pointing to a Type that does NOT correspond to a class in the Ontology; otherwise, this relation should be an ObjectProperty relation.")]
    public class DataProperty : ClassRelation 
    {
        public Type DomainClass { get; set; }

        public Type RangeType { get; set; } // In a DataProperty, the range should NOT correspond to an Ontology Class.
    }
}
