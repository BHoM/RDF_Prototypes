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
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using BH.Adapters;
using BH.Engine.Adapters.TTL;
using Log = BH.Engine.Adapters.RDF.Log;

namespace BH.Adapters.TTL
{
    public partial class TTLAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public override IEnumerable<object> Pull(oM.Data.Requests.IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!string.IsNullOrEmpty(m_filepath))
            {
                Log.RecordNote($"Pulling objects from file: {m_filepath}.");
                return new List<object>() { BH.Engine.Adapters.TTL.Convert.FromTTL(m_filepath) };
            }

            Log.RecordNote($"To pull from a TTL file, please specify the filepath in the TTL Adapter." +
                $"If you want to convert a TTL text string to objects, please use the method {nameof(BH.Engine.Adapters.TTL.Convert.FromTTL)} ");

            return new List<object>();
        }
    }
}