using BH.Engine.RDF.Types;
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
            tBoxSettings = tBoxSettings ?? new TBoxSettings();

            if (!tBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)
                return obj.GetType();

            if (obj.IsCustomObjectWithTypeKey(tBoxSettings))
                return new CustomObjectType((CustomObject)obj, tBoxSettings);

            return obj.GetType();
        }
    }
}
