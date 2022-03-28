
using BH.oM.Base;
using BH.oM.RDF;
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
        public static void RelationsFromType(this Type oMType, List<IRelation> resultRelations, int recursionLevel = 0, int maxRecursion = int.MaxValue)
        {
            PropertyInfo[] properties = null;

            // Only get declared properties.
            properties = oMType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            IRelation propertyRelation = null;
            foreach (PropertyInfo property in properties)
            {
                if (oMType.IsInterface)
                    propertyRelation = new RequiresProperty() { Subject = oMType, Object = property };
                else
                    propertyRelation = new HasProperty<Type, PropertyInfo>() { Subject = oMType, Object = property };

                resultRelations.Add(propertyRelation);
            }

            Type[] implementedInterfaces = oMType.GetInterfaces();
            foreach (Type implementedInterface in implementedInterfaces)
            {
                propertyRelation = new IsA() { Subject = oMType, Object = implementedInterface };

                resultRelations.Add(propertyRelation);
            }

            Type baseType = oMType.BaseType;
            if (baseType != null)
                resultRelations.Add(new IsSubclassOf<Type, Type>() { Subject = oMType, Object = baseType });
        }
    }
}
