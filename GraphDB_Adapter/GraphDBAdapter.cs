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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Adapters.GraphDB;
using BH.oM.Adapters.RDF;

namespace BH.Adapter.GraphDB
{
    public partial class GraphDBAdapter : BHoMAdapter
    {
        private string m_graphDBexePath;
        private readonly string m_repositoryName;
        private string m_serverAddress;
        private GraphSettings m_graphSettings;
        private LocalRepositorySettings m_localRepositorySettings;

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for GraphDB.")]
        [Input("graphDBexePath", "Input filepath to GraphDB.exe on your machine.")]
        [Input("repositoryName", "Name of the GraphDB repository where to store and retrieve data.")]
        [Input("serverAddress", "Localhost address where GraphDB is exposed. This can be changed from GraphDB settings file.")]
        [Input("graphSettings", "Settings for the graph creation.")]
        [Input("activate", "To start GraphDB, switch toggle to True.")]
        [Output("The created GraphDB adapter.")]
        public GraphDBAdapter(string graphDBexePath = "%APPDATA%\\..\\Local\\GraphDB Free\\GraphDB Free.exe", 
            string repositoryName = "BHoMGraphDBRepo",
            string serverAddress = "http://localhost:7200/", 
            GraphSettings graphSettings = null,
            bool activate = false)
        {
            // The Adapter constructor can be used to configure the Adapter behaviour.
            // For example:
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly; // Adapter `Push` Action simply calls "Create" method.

            if (activate)
                BH.Engine.Adapters.GraphDB.Compute.StartGraphDBProcess(graphDBexePath, true);

            this.m_graphDBexePath = graphDBexePath;
            this.m_repositoryName = repositoryName;
            this.m_serverAddress = serverAddress;
            this.m_graphSettings = graphSettings ?? new GraphSettings();
            this.m_localRepositorySettings = new LocalRepositorySettings();
        }

        // You can add any other constructors that take more inputs here. 

        /***************************************************/
        /**** Private  Fields                           ****/
        /***************************************************/

        // You can add any private variable that should be in common to any other adapter methods here.
        // If you need to add some private methods, please consider first what their nature is:
        // if a method does not need any external call (API call, connection call, etc.)
        // we place them in the Engine project, and then reference them from the Adapter.
        // See the wiki for more information.

        /***************************************************/
    }
}

