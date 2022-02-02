using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public interface IRelation
    {
        object Subject { get; set; }
        object Object { get; set; }
    }
}
