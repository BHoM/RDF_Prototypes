using BH.Engine.Base;
using BH.oM.RDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log = BH.Engine.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Modify
    {
    //    public static CSharpGraph AddRangeObjectProperty(CSharpGraph cSharpGraph, DataProperty dataPropertyToModify)

    //    {
    //        CSharpGraph cSharpGraph_deepclone = cSharpGraph.DeepClone();

    //        DataProperty dtProp = cSharpGraph_deepclone.DataProperties.Where(op => op.Equals(dataPropertyToModify)).FirstOrDefault();
    //        if (dtProp == null)
    //        {
    //            Log.RecordError($"Could not find DataProperty of name {dataPropertyToModify}.");
    //            return cSharpGraph_deepclone;
    //        }

    //        dtProp.OWLDataPropertyType = owlDataPropertyType;

    //        return cSharpGraph_deepclone;
    //    }
    }
}
