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
        public static List<IRelation> RelationsFromType(this Type type, bool recurse = false)
        {
            List<IRelation> resultRelations = new List<IRelation>();

            if (!type.IsBHoMType())
                return new List<IRelation>();

            PropertyInfo[] properties = null;
            properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            IRelation propertyRelation = null;
            foreach (PropertyInfo pInfo in properties)
            {
                if (type.IsInterface)
                    propertyRelation = new RequiresProperty() { Subject = type, Object = pInfo };
                else
                    propertyRelation = new HasProperty() { Subject = type, Object = pInfo };

                resultRelations.Add(propertyRelation);

                // Recurse for each property
                if (recurse)
                    resultRelations.AddRange(pInfo.PropertyType.RelationsFromType());
            }

            Type[] implementedInterfaces = type.GetInterfaces();
            foreach (Type implementedInterface in implementedInterfaces)
            {
                propertyRelation = new IsA() { Subject = type, Object = implementedInterface };

                resultRelations.Add(propertyRelation);

                // Recurse for each implemented interface
                if (recurse)
                    resultRelations.AddRange(implementedInterface.RelationsFromType());
            }

            Type baseType = type.BaseType;
            if (baseType != null)
            {
                resultRelations.Add(new IsSubclassOf() { Subject = type, Object = baseType });

                // Recurse for the base type
                if (recurse)
                    resultRelations.AddRange(baseType.RelationsFromType());
            }

            return resultRelations;
        }
    }
}
