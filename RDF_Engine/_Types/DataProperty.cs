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

using AngleSharp.Dom;
using BH.Engine.Adapters.RDF;
using BH.Engine.Base;
using BH.oM.Base;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.oM.Adapters.RDF
{
    [Description("Identifies a relation between two Types in a CSharp graph that is akin to a Data Property relation in an Ontology format." +
        "The RangeType should be pointing to a Type that does NOT correspond to a class in the Ontology; otherwise, this relation should be an ObjectProperty relation.")]
    public class DataProperty : ClassRelation, IDataProperty, IImmutable
    {
        public DataProperty(Type domainClass, Type rangeType, PropertyInfo pi, TBoxSettings tBoxSettings)
        {
            if (rangeType.IsOntologyClass(tBoxSettings) || rangeType.IsListOfOntologyClasses(tBoxSettings))
                Log.RecordError("Cannot create a DataProperty with a RangeType that is a type corresponding to an Ontology Class or a List of Ontology Classes.", typeof(ArgumentException));

            RangeType = rangeType;
            DomainClass = domainClass;
            PropertyInfo = pi;
        }
    }
}
