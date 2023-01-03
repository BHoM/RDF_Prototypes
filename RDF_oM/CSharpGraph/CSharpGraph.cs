using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Graph obtained from a collection of CSharp objects via Reflection (metaprogramming)." +
        "The graph is structured in a way that no reflection should be needed when translating it to an Ontology format, e.g. TTL.")]
    public class CSharpGraph : IObject 
    {
        [Description("CSharp Types that will correspond to ontology Classes. This is part of the T-Box.")]
        public HashSet<Type> Classes { get; set; } = new HashSet<Type>();

        [Description("Relations between Classes that will correspond to Object Properties when translating to an Ontology format. This is part of the T-Box.")]
        public HashSet<ObjectProperty> ObjectProperties { get; set; } = new HashSet<ObjectProperty>();

        [Description("Relations between Classes that will correspond to Data Properties when translating to an Ontology format. This is part of the T-Box.")]
        public HashSet<DataProperty> DataProperties { get; set; } = new HashSet<DataProperty>();

        [Description("CSharp objects for which the T-Box relations and classes were defined. This is part of the A-Box.")]
        public HashSet<object> AllIndividuals { get; set; } = new HashSet<object>();

        [Description("Relations between the objects instances based on the T-Box relations and classes. This is part of the A-Box.")]
        public HashSet<IndividualRelation> IndividualRelations { get; set; } = new HashSet<IndividualRelation>();

        [Description("Settings used to compose this Graph ontology.")]
        public OntologySettings OntologySettings { get; set; }
    }
}
