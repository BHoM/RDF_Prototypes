using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public class IsSubclassOf : IRelation
    {
        [Description("The parent.")]
        public object Subject { get; set; }
        [Description("The child.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}
