using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    // Subject, Predicate, Object

    public class BHoMTriple : IRelation
    {
        public object Subject { get; set; }
        public Predicates Predicate { get; set; } = Predicates.HasElement;
        public object Object { get; set; }
    }

    public enum Predicates
    {
        HasElement,
        IsSubClass,
        IsParent,
    }
}
