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

using BH.Adapters.TTL;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.oM.Adapters.RDF;
using System.Data.Common;

namespace BH.Test.RDF
{
    /// <summary>
    /// Base class for tests, with customised settings.
    /// </summary>
    public abstract partial class Test
    {
        /// <summary>
        /// To be initialized in Test SetUp methods.
        /// </summary>
        public static TTLAdapter m_adapter;

        /// <summary>
        /// To be initialized in Test SetUp methods.
        /// </summary>
        public static OntologySettings m_ontologySettings = null;

        /// <summary>
        /// Required because RandomObject generally fails with Column, returning invalid customdata key names.
        /// </summary>
        protected static Column CreateRandomColumn()
        {
            Column randomColumn = new Column();
            randomColumn.Name = "A column object";
            randomColumn.Property = new ConstantFramingProperty() { Material = new oM.Physical.Materials.Material() { Name = "SomeMaterial" } };
            randomColumn.Location = BH.Engine.Adapters.RDF.Testing.Create.RandomObject<Arc>();
            return randomColumn;
        }
    }
}
