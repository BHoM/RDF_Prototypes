using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.RDF
{
    [Description("Base interface for classes representing relations between individuals in a CSharpGraph.")]
    public interface IndividualRelation // We do not want to implement the IObject interface on this type: no need to expose this to the UI, other than as an output from an `Explode`d CSharpGraph.
    {
        object Individual { get; set; }
    }
}
