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

using BH.Engine.Adapters.RDF;
using BH.Engine.Adapters.RDF.Types;
using BH.oM.Base;
using System;
using System.ComponentModel;
using System.Reflection;

namespace BH.oM.Adapters.RDF
{
    [Description("Identifies a relation between two Types in a CSharp graph that is akin to an Object Property relation in an Ontology format." +
        "If the Range class is set to a Type that is another class in the Ontology, the ObjectProperty relation can be seen as a 'HasProperty' relation.")]
    public class ObjectProperty : ClassRelation, IObjectProperty, IImmutable // aka "HasProperty" when the range is another class in the Ontology.
    {
        public ObjectProperty(Type domainClass, Type rangeClass, PropertyInfo propertyInfo, TBoxSettings tBoxSettings)
        {
            if (!(propertyInfo is CustomPropertyInfo) &&
                !rangeClass.IsOntologyClass(tBoxSettings) &&
                !rangeClass.IsListOfOntologyClasses(tBoxSettings))
                Log.RecordError("Cannot create an ObjectProperty with a RangeType that is not a type corresponding to an Ontology Class or to a List of Ontology classes.", typeof(ArgumentException));

            DomainClass = domainClass;
            RangeType = rangeClass;

            // TODO: we currently rely on the order of property creation in order to ensure the Pi is correct (DomainClass pointing to the basemost type).
            // In theory, we should be able to do the following without any issue, but it would require a large refactor:
            //   var baseMostProp = this.DomainClass.BaseTypes().SelectMany(t => t.GetProperties()).FirstOrDefault(p => p.Name == pi.Name);
            //   PropertyInfo = baseMostProp;
            // More precisely, we should be able to construct any class relation with just the Domain Type, the Range Type, and a property name (not a piInfo).

            PropertyInfo = propertyInfo;
        }
    }
}
