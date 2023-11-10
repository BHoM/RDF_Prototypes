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
    [Description("Identifies a Class Relation between two Types in a CSharpGraph that is akin to a Data Property relation in an Ontology.\n" +
        "The RangeType must be pointing to a Type that does NOT correspond to a Class in the Ontology " +
        "(otherwise it would be an ObjectProperty relation).")]
    public class DataProperty : ClassRelation, IDataProperty, IImmutable
    {
        public DataProperty(Type domainClass, Type rangeType, PropertyInfo pi, TBoxSettings tBoxSettings)
        {
            if (rangeType.IsOntologyClass(tBoxSettings) || rangeType.IsListOfOntologyClasses(tBoxSettings))
            {
                Log.RecordError($"Cannot create a DataProperty for the property {pi.FullNameValidChars()} " +
                       $"because the provided RangeType is {rangeType.FullNameValidChars()}, which is a type corresponding to an Ontology Class or to a List of Ontology classes.", typeof(ArgumentException));
                
                return;
            }

            RangeType = rangeType;
            DomainClass = domainClass;

            // TODO: we currently rely on the order of property creation in order to ensure the Pi is correct (DomainClass pointing to the basemost type).
            // In theory, we should be able to do the following without any issue, but it would require a large refactor:
            //   var baseMostProp = this.DomainClass.BaseTypes().SelectMany(t => t.GetProperties()).FirstOrDefault(p => p.Name == pi.Name);
            //   PropertyInfo = baseMostProp;
            // More precisely, we should be able to construct any class relation with just the Domain Type, the Range Type, and a property name (not a piInfo).

            PropertyInfo = pi;
        }
    }
}
