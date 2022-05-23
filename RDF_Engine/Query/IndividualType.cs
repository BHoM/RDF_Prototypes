using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Type IndividualType(this object obj, TBoxSettings tBoxSettings)
        {
            if (tBoxSettings == null || !tBoxSettings.TreatCustomObjectsWithTypeKeyAsTypes)
                return obj.GetType();

            if (obj.IsCustomObjectWithTypeKey())
                return new CustomType((CustomObject)obj, tBoxSettings, tBoxSettings.CustomobjectsTypeKey);

            return obj.GetType();
        }
    }
}
