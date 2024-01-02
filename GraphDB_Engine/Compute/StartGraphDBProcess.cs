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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.GraphDB
{
    public static partial class Compute
    {
        [Description("Starts a GraphDBProcess on your machine.")]
        [Input("graphDBexePath", "Input filepath to GraphDB.exe on your machine.")]
        [Input("run", "To start GraphDB switch toggle to True.")]
        public static void StartGraphDBProcess(string graphDBexePath = "%APPDATA%\\Local\\GraphDB Free\\GraphDB Free.exe", bool run = false)
        {
            if (!System.IO.File.Exists(graphDBexePath))
            {
                Log.RecordError($"Could not find the {nameof(graphDBexePath)}. Please make sure that the path to the GraphDB executable file is correct.", typeof(ArgumentException));
                return;
            }

            if (run == true)
            {
                Process.Start(graphDBexePath);
                //Task.Delay(500).Wait();
            }
        }
    }
}

