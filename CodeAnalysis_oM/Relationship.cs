using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.External.RDF
{
    public class Relationship
    {
        public virtual string Relation { get; set; } = "HasElement";
        public virtual object From { get; set; } // Room
        public virtual object To { get; set; } // Column
    }
}
