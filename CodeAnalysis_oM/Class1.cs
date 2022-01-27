using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.External.RDF
{
    public class RDF
    {
        public virtual Type Type { get; set; }
        public List<Type> ParentBHoMTypes { get; set; }
        public List<Type> PublicMembers { get; set; }


    }
}
