using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Describes that a thing is equivalent to another thing. Typically used to describe C# interface implementations.")]
    public class IsA : IRelation
    {
        [Description("Thing that owns a certain property.")]
        public object Subject { get; set; }
        [Description("The property owned by the thing.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}
