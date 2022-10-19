using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Can be used to describe that a thing is list of classes.")]
    public class IsAListOf : IDirectionalRelation
    {
        [Description("Thing that is a list of classes.")]
        public object Subject { get; set; }
        [Description("Class whose type makes the list.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}
