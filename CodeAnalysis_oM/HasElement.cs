using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.External.RDF
{
    // Subject, Predicate, Object

    public class HasElement<TSubject, TObject>
    {
        public virtual TSubject Subject { get; set; } 
        public virtual TObject Object { get; set; }
    }
}
