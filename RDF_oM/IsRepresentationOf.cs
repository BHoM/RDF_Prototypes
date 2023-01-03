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
    public class IsRepresentationOf : IRelation
    {
        public List<ObjectIdentity> Set1 { get; set; }
        public List<ObjectIdentity> Set2 { get; set; }
        public ComparisonConfig ComparisonConfig { get; set; }
        bool IsBidirectional { get; set; } = true;
    }

    public class ObjectIdentity : IObject
    {
        public string Hash { get; set; }
        public double[] GeometryHash { get; set; }
    }
}
