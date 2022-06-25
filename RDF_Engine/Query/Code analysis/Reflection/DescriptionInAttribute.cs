using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Returns the text included in the DescriptionAttribute of a Type, if present.")]
        public static string DescriptionInAttribute(this Type t)
        {
            try
            {
                CustomAttributeData classDescriptionAttribute = t.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DescriptionAttribute));
                string classDescription = classDescriptionAttribute?.ConstructorArguments.FirstOrDefault().Value.ToString();

                return classDescription;
            }
            catch
            {
                return "";
            }
        }

        [Description("Returns the text included in the DescriptionAttribute of a PropertyInfo, if present.")]
        public static string DescriptionInAttribute(this PropertyInfo pi)
        {
            try
            {
                CustomAttributeData descriptionAttribute = pi.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DescriptionAttribute));
                string description = descriptionAttribute?.ConstructorArguments.FirstOrDefault().Value.ToString();

                return description;
            }
            catch
            {
                return "";
            }
        }
    }
}
