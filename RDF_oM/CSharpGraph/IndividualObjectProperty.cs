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

namespace BH.oM.RDF
{
    public class IndividualObjectProperty : IndividualRelation
    {
        // Each individual needs to link to another individual if it has properties or is owned by another object.
        public object Individual { get; set; }
        public object RangeIndividual { get; set; }

        // Class relation corresponding to these Individuals' relation.
        public ObjectProperty HasProperty { get; set; }

        public override bool Equals(object obj)
        {
            IndividualObjectProperty o = obj as IndividualObjectProperty;
            if (o == null)
                return false;

            return Individual == o.Individual && RangeIndividual == o.RangeIndividual &&
                HasProperty.DomainClass == o.HasProperty.DomainClass && HasProperty.RangeClass == o.HasProperty.RangeClass;
        }

        public override int GetHashCode()
        {
            int A = Individual.GetHashCode();
            int? B = RangeIndividual?.GetHashCode();
            int C = HasProperty.DomainClass.GetHashCode();
            int D = HasProperty.RangeClass.GetHashCode();
            return A + B ?? 0 + C + D;
        }
    }
}
