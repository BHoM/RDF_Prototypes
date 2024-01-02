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
    [Description("Base abstract class for CSharpGraph's Class Relations (either Object or Data Properties)." +
        "This abstract class includes comparison logic that concrete Class Relations must have.")]
    public abstract class ClassRelation : IClassRelation, IImmutable // We do not want to implement the IObject interface on this type: no need to expose this to the UI, other than as an output from an `Explode`d CSharpGraph.
    {
        [Description("CSharp PropertyInfos can be seen as the correspondant to Ontology Object Properties." +
            "The propertyInfo must be the one used to construct this relation. Its DeclaringType may differ from the ClassRelation's DomainClass.")]
        public PropertyInfo PropertyInfo { get; protected set; }

        [Description("Type representing the Domain (origin) of the Class Relation. The Domain type will always correspond to an Ontology Class for both Object Properties and Data Properties relations.")]
        public Type DomainClass { get; protected set; }

        [Description("Type representing the Range (destination) of the Class Relation. " +
            "The Range type will correspond to an Ontology Class if the Relation is an Object Property," +
            "and to a non-Ontology Class (Data Type) if the Relation is an Data Property.")]
        public Type RangeType { get; protected set; }

        [Description("The Equals method must consider equal two Relations whose Domain class can be assigned to the other's (other than having the same RangeType and Property Name). " +
            "This is because the Relation should be always considered as originating from the basemost type of the Domain class." +
            "For this reason, this method is sealed.")]
        public override sealed bool Equals(object obj)
        {
            ClassRelation clRel = obj as ClassRelation;
            if (clRel == null)
                return false;

            if (clRel.RangeType != this.RangeType)
                return false;

            if (DomainClass.IsAssignableFromIncludeGenerics(clRel.DomainClass) || clRel.DomainClass.IsAssignableFromIncludeGenerics(DomainClass))
                return true;

            return false;
        }

        [Description("The HashCode method must consider equal two Relations whose Domain class can be assigned to the other's (other than having the same RangeType and Property Name). " +
            "This is because the Relation should be always considered as originating from the basemost type of the Domain class." +
            "For this reason, this method is sealed.")]
        public override sealed int GetHashCode()
        {
            unchecked
            {
                int hash = RangeType.GetHashCode() + this.PropertyInfo.Name.GetHashCode();

                var parentProp = this.DomainClass.BaseTypes().SelectMany(t => t.GetProperties()).FirstOrDefault(p => p.Name == this.PropertyInfo.Name);

                if (parentProp != null)
                    return hash + parentProp.DeclaringType.GetHashCode();
                else
                    return hash + this.DomainClass.GetHashCode();
            }
        }
    }
}

