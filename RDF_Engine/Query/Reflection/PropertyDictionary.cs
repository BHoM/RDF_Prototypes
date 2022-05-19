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
        public static Dictionary<string, object> PropertyDictionary(this object obj, bool onlyDeclaredProps = true)
        {
            if (obj == null)
            {
                Base.Compute.RecordWarning("Cannot query the property dictionary of a null object. An empty dictionary will be returned.");
                return new Dictionary<string, object>();
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();

            PropertyInfo[] properties;

            if (onlyDeclaredProps)
                properties = obj.GetType().DeclaredProperties();
            else
                properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;
                var value = prop.GetValue(obj, null);
                dic[prop.Name] = value;
            }

            return dic;
        }
    }
}