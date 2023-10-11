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
using BH.Engine.Adapters.RDF;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapters;
using Log = BH.Engine.Adapters.RDF.Log;
using Compute = BH.Engine.Adapters.GraphDB.Compute;
using BH.Adapters.TTL;
using System.IO;

namespace BH.Adapter.GraphDB
{
    public partial class GraphDBAdapter6 : BHoMAdapter
    {
        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.UpdateOrCreateOnly, ActionConfig actionConfig = null)
        {

            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string TTLfilepath = Path.Combine(userDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_GraphDBPush.ttl");

            // Creates a Turtle file.
            TTLAdapter ttlAdapter = new TTLAdapter(TTLfilepath, m_graphSettings, m_localRepositorySettings);
            ttlAdapter.Push(objects);


            // Posts the content of the Turtle file to GraphDB.
            Compute.PostToRepo(TTLfilepath, m_username, m_serverAddress, m_repositoryName, m_graphName, false, true);

            return objects.ToList();
        }
    }
}