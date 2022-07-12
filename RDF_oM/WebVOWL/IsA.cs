using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Can be used to describe that a thing belongs to a class; or that a thing is equivalent to another thing. " +
        "When used for BHoM types, this describes C# interface implementation.")]
    public class IsA : IRelation
    {
        [Description("Thing that can be included in the class, or that is equivalent to something else.")]
        public object Subject { get; set; }
        [Description("The class that can include the thing, or another thing that is equivalent to the thing.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}
