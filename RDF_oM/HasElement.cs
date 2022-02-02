using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public class HasElement : IRelation
    {
        [Description("Element that owns by the other element.")]
        public object Subject { get; set; }
        [Description("Element that is owned by the other element.")]
        public object Object { get; set; }
    }
}
