using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    [Description("Classes implementing this interface represent a relation between a Subject thing and an Object thing." +
        "The class name will represent the relation predicate.")]
    public interface IRelation : IObject
    {
        // We use `System.Object` as a type, instead of `System.Type`, because this way we can use IRelations for:
        // - Ontological relations, i.e. between C# classes. We can call these "Static relations"
        // - Knowledge graph relations, i.e. between C# objects (instances of classes). We can call these "Dynamic relations".
        object Subject { get; set; } 
        object Object { get; set; }

        bool IsBidirectional { get; set; } // If unset, this defaults to false.
    }
}
