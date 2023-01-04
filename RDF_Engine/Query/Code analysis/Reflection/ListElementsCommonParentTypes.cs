/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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