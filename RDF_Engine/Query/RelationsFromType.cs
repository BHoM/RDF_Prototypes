
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
        public static List<IRelation> RelationsFromType(this Type oMType, List<IRelation> existingRelations = null, bool onlyDeclaredProperties = true)
        {
            List<IRelation> resultRelations = new List<IRelation>();
            if (existingRelations != null)
                resultRelations.AddRange(existingRelations);

            PropertyInfo[] properties = null;

            if (onlyDeclaredProperties)
                properties = oMType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            else
                properties = oMType.GetProperties();

            IRelation propertyRelation = null;
            foreach (PropertyInfo property in properties)
            {
                if (oMType.IsInterface)
                    propertyRelation = new RequiresProperty() { Subject = oMType, Object = property };
                else
                    propertyRelation = new HasProperty() { Subject = oMType, Object = property };

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
                resultRelations.Add(new IsSubclassOf() { Subject = oMType, Object = baseType });

            return resultRelations;
        }
    }
}
