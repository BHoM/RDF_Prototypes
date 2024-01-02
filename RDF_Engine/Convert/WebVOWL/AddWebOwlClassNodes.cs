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
using BH.oM.Adapters.RDF;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Convert
    {
        private static string AddWebOwlClassNodes(Type type, JArray classArray, JArray classAttributeArray, HashSet<string> addedWebVOWLNodeIds, LocalRepositorySettings settings, HashSet<string> internalNamespaces = null)
        {
            string typeId = type.UniqueNodeId();

            if (addedWebVOWLNodeIds.Contains(typeId))
                return typeId;

            Uri typeUri = type.OntologyUri(new TBoxSettings(), settings);
            string comment = type.DescriptionInAttribute();
            bool isExternal = !type.IsInNamespace(internalNamespaces) ?? false;

            // 1) CLASS
            classArray.AddToIdTypeArray(typeId, "owl:Class");

            // 2) CLASS ATTRIBUTE
            classAttributeArray.AddToAttributeArray(typeId, typeUri, type.DescriptiveName(true), isExternal, new List<string>() { "object" }, comment);

            addedWebVOWLNodeIds.Add(typeId);

            return typeId;
        }

        private static string AddWebOwlClassNodes(PropertyInfo pInfo, JArray classArray, JArray classAttributeArray, HashSet<string> addedWebVOWLNodeIds, LocalRepositorySettings settings)
        {
            string propertyNodeId = pInfo.UniqueNodeId();

            // Check if we need to add a class node for the property type.
            if (!addedWebVOWLNodeIds.Contains(propertyNodeId))
            {

                // 1) CLASS
                classArray.AddToIdTypeArray(propertyNodeId, "owl:Class");

                // 2) CLASS ATTRIBUTE
                classAttributeArray.AddToAttributeArray(propertyNodeId, pInfo.OntologyURI(new TBoxSettings(), settings), pInfo.DescriptiveName());

                addedWebVOWLNodeIds.Add(propertyNodeId);
            }

            return propertyNodeId;
        }
    }
}
