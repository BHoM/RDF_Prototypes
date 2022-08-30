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



   
        public static CSharpGraph ObjectProperty(CSharpGraph cSharpGraph, ObjectProperty objectPropertyToModify, OWLObjectPropertyType owlObjectPropertyType)
        {
            CSharpGraph cSharpGraph_deepclone = cSharpGraph.DeepClone();

            ObjectProperty objProp = cSharpGraph_deepclone.ObjectProperties.Where(op => op.Equals(objectPropertyToModify)).FirstOrDefault();
            if (objProp == null)
            {
                Log.RecordError($"Could not find ObjectProperty of name {objectPropertyToModify}.");
                return cSharpGraph_deepclone;
            }

            objProp.OWLObjectPropertyType = owlObjectPropertyType;

            return cSharpGraph_deepclone;
        }
    }
}
