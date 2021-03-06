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
        public static bool IsCustomObjectWithTypeKey(this object obj, TBoxSettings tBoxSettings)
        {
            return obj.IsCustomObjectWithTypeKey(tBoxSettings.CustomobjectsTypeKey);
        }

        public static bool IsCustomObjectWithTypeKey(this object obj, string typeKey)
        {
            CustomObject co = obj as CustomObject;
            if (co != null)
                return co.CustomData.Keys.Contains(typeKey);

            return false;
        }
    }
}
