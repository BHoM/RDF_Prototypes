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

using BH.Engine.Adapters.RDF;
using System.Linq;
using System.Reflection;

namespace BH.oM.Adapters.RDF
{
    public class IndividualDataProperty : IIndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object Value { get; set; }

        // PropertyInfo that generated this Data property of this individual.
        public PropertyInfo PropertyInfo { get; set; }

        public override bool Equals(object obj)
        {
            IndividualDataProperty o = obj as IndividualDataProperty;
            if (o == null)
                return false;

            if (!Individual.Equals(o.Individual))
                return false;

            if (!((Value != null && Value.Equals(o.Value)) || (Value == null && o.Value == null)))
                return false;

            if (!PropertyInfo.Name.Equals(o.PropertyInfo.Name))
                return false;

            if (PropertyInfo.PropertyType == o.PropertyInfo.PropertyType)
                return true;

            if (PropertyInfo.DeclaringType.IsAssignableFrom(o.PropertyInfo.DeclaringType))
                return true;

            if (o.PropertyInfo.DeclaringType.IsAssignableFrom(PropertyInfo.DeclaringType))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int A = Individual.GetHashCode();
                int B = Value?.GetHashCode() ?? 0;
                int C = PropertyInfo.PropertyType.GetHashCode();
                int D = PropertyInfo.Name.GetHashCode();

                var parentProp = PropertyInfo.DeclaringType.BaseTypes().SelectMany(t => t.GetProperties()).FirstOrDefault(p => p.Name == this.PropertyInfo.Name);

                if (parentProp != null)
                    return A + B + C + D + parentProp.DeclaringType.GetHashCode();
                else
                    return A + B + C + D + PropertyInfo.DeclaringType.GetHashCode();

            }
        }
    }
}

