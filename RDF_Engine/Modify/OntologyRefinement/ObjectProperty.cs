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
        public static CSharpGraph ObjectProperty(CSharpGraph cSharpGraph, ObjectProperty objectPropertyToModify, OWLObjectPropertyType owlObjectPropertyType)
        {
            CSharpGraph cSharpGraph_deepclone = cSharpGraph.DeepClone();

            for (int i = 0; i < cSharpGraph_deepclone.ObjectProperties.Count; i++)
            {
                var objPropert = cSharpGraph_deepclone.ObjectProperties.ElementAtOrDefault(i);

                if (objPropert == null || !objPropert.Equals(objectPropertyToModify))
                    continue;

                OWLObjectProperty oWLObjectProperty = new OWLObjectProperty();
                oWLObjectProperty.DomainClass = objPropert.DomainClass;
                oWLObjectProperty.RangeClass = objPropert.RangeClass;
                oWLObjectProperty.PropertyInfo = objPropert.PropertyInfo;

                oWLObjectProperty.OWLObjectPropertyType = owlObjectPropertyType;

                cSharpGraph_deepclone.ObjectProperties[i] = oWLObjectProperty;

                return cSharpGraph_deepclone;
            }

            Log.RecordError($"Could not find ObjectProperty of name {objectPropertyToModify}.");
            return cSharpGraph;
        }
    }
}
