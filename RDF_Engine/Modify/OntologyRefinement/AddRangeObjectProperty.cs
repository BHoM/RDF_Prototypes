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
        public static CSharpGraph AddRangeObjectProperty(CSharpGraph cSharpGraph, ObjectProperty objectPropertyToModify, Type range)
        {
            CSharpGraph cSharpGraph_deepclone = cSharpGraph.DeepClone();

            ObjectProperty newProp = new ObjectProperty() { DomainClass = objectPropertyToModify.DomainClass, RangeClass = range };

            cSharpGraph_deepclone.ObjectProperties.Add(newProp);

            return cSharpGraph_deepclone;
        }
    }
}
