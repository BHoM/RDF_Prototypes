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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Adapters.RDF;
using NUnit.Framework;
using Shouldly;

namespace RDF_Tests.GraphDB
{
    internal class GraphDBTests
    {
        //[Test] // Restore when fixed as below.
        public void PullFromRepoTest()
        {
            // This test should be corrected to first verify if the connectivity to GraphDB is available.
            // If not available, the test should not run, to avoid false failures. Commenting out the code below.

            var result = BH.Engine.Adapters.GraphDB.Compute.PullFromRepo(repositoryName:"myrepo123", run:true);
            result.ShouldNotBeNull();
        }
    }
}
