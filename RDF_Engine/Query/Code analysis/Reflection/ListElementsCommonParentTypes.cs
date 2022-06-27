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
        [Description("Find the distinct parent types that are common to all elements of the input list.")]
        public static HashSet<Type> ListElementsCommonParentTypes(this List<object> objList)
        {
            HashSet<Type> commonParentTypes = new HashSet<Type>();

            List<HashSet<Type>> objsParentTypes = new List<HashSet<Type>>();
            foreach (var obj in objList)
            {
                HashSet<Type> listObjParentTypes = new HashSet<Type>();

                Type objType = obj.GetType();
                listObjParentTypes.Add(objType.DeclaringType);

                var implementedInterfaces = objType.GetInterfaces().ToList();
                implementedInterfaces.ForEach(i => listObjParentTypes.Add(i));

                objsParentTypes.Add(listObjParentTypes);
            }

            // Check if all objects have common parent types between each other
            foreach (HashSet<Type> objParentTypes in objsParentTypes)
            {
                foreach (Type objParentType in objParentTypes)
                {
                    bool addType = true;

                    IEnumerable<HashSet<Type>> allOtherObjsParentTypes = objsParentTypes.Where(h => h != objParentTypes);
                    foreach (var otherObjsParentTypes in allOtherObjsParentTypes)
                    {
                        if (!otherObjsParentTypes.Contains(objParentType))
                        {
                            addType = false;
                            break;
                        }
                    }

                    if (addType)
                        commonParentTypes.Add(objParentType);
                }
            }

            return commonParentTypes;
        }
    }
}