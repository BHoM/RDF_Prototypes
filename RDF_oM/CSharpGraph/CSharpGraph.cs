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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapters.RDF
{
    [Description("Graph obtained from a collection of CSharp objects via Reflection (metaprogramming)." +
        "The graph is structured in a way that no reflection should be needed when translating it to an Ontology format, e.g. TTL.")]
    public class CSharpGraph : IObject 
    {
        [Description("CSharp Types that will correspond to ontology Classes. This is part of the T-Box.")]
        public HashSet<Type> Classes { get; set; } = new HashSet<Type>();

        [Description("Relations between Classes that will correspond to Object Properties when translating to an Ontology format. This is part of the T-Box.")]
        public HashSet<IObjectProperty> ObjectProperties { get; set; } = new HashSet<IObjectProperty>();

        [Description("Relations between Classes that will correspond to Data Properties when translating to an Ontology format. This is part of the T-Box.")]
        public HashSet<IDataProperty> DataProperties { get; set; } = new HashSet<IDataProperty>();

        [Description("CSharp objects for which the T-Box relations and classes were defined. This is part of the A-Box.")]
        public HashSet<object> AllIndividuals { get; set; } = new HashSet<object>();

        [Description("Relations between the objects instances based on the T-Box relations and classes. This is part of the A-Box.")]
        public HashSet<IIndividualRelation> IndividualRelations { get; set; } = new HashSet<IIndividualRelation>();

        [Description("Settings used to compose this Graph ontology.")]
        public GraphSettings GraphSettings { get; set; }
    }
}
