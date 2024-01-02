/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.RDF.Testing
{
    public static partial class Create
    {
        public static List<T> RandomObjects<T>(int count = 100, bool assignIdFragmentWithProgressiveId = false, bool assignObjectName = false, string objectNamePrefix = "bar_") where T : IObject
        {
            return RandomObjects(typeof(T), count, assignIdFragmentWithProgressiveId, assignObjectName, objectNamePrefix).OfType<T>().ToList();
        }

        public static List<IBHoMObject> RandomObjects(Type t, int count = 100, bool assignHashWithProgressiveNumber = false, bool assignObjectName = false, string objectNamePrefix = "bar_")
        {
            List<IBHoMObject> objs = new List<IBHoMObject>();

            for (int i = 0; i < count; i++)
            {
                IObject obj = BH.Engine.Base.Create.RandomObject(t);

                if (assignHashWithProgressiveNumber)
                {
                    IBHoMObject bhomObj = obj as IBHoMObject;
                    bhomObj = bhomObj.AddFragment(new HashFragment() { Hash = i.ToString() });
                    obj = bhomObj;
                }

                if (assignObjectName)
                {
                    IBHoMObject bhomObj = obj as IBHoMObject;
                    bhomObj.Name = objectNamePrefix + i.ToString();
                    obj = bhomObj;
                }

                objs.Add(obj as dynamic);
            }

            return objs;
        }
    }
}



