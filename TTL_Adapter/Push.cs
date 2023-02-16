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

using BH.Adapter;
using BH.Adapters.TTL;
using BH.Engine.RDF;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using BH.Adapters;
using Log = BH.Engine.RDF.Log;
using Convert = BH.Engine.Adapters.TTL.Convert;
using Compute = BH.Engine.Adapters.TTL.Compute;

namespace BH.Adapters.TTL
{
    public partial class TTLAdapter : BHoMAdapter
    {
        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!string.IsNullOrEmpty(m_filepath))
            {
                Compute.TTLGraph(objects.ToList(), m_filepath, m_ontologySettings, m_localRepositorySettings);
                return new List<object>() { $"The objects have been written to TTL file at filepath: {m_filepath}" };
            }

            return new List<object>() { Compute.TTLGraph(objects.ToList(), m_ontologySettings, m_localRepositorySettings) };
        }
    }
}