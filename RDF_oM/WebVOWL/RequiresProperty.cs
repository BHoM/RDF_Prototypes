using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Describes a property that subclasses of a thing must own. Typically represents properties of a C# interface.")]
    public class RequiresProperty : IRelation
    {
        [Description("Thing that requires any subclass of itself to own a certain property.")]
        public object Subject { get; set; }
        [Description("The property that must be owned by subclasses of the thing.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}
