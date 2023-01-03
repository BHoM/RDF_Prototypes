using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public class EquivalentRepresentation : IRelation
    {
        [Description("Set of objects that have an equivalent representation in the objects in set 2.")]
        public List<ObjectIdentity> Set1 { get; set; }

        [Description("Set of objects that have an equivalent representation in the objects in set 1.")]
        public List<ObjectIdentity> Set2 { get; set; }

        [Description("ComparisonConfing under which the equivalency of representation holds.")]
        public ComparisonConfig ComparisonConfig { get; set; }

        [Description("Whether the equivalency of representation works in both ways. If false, only set1 to set2 is assumed.")]
        bool IsBidirectional { get; set; } = true;
    }

    public class ObjectIdentity : IObject
    {
        public string Hash { get; set; }
        public double[] GeometryHash { get; set; }
    }
}
