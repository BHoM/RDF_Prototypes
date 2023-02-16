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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using Log = BH.Engine.Adapters.RDF.Log;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        [Description("Attempts to parse the input string with a TTL parser and get the individuals.")]
        public static List<OntologyResource> Individuals(this string TTLGraph)
        {
            OntologyGraph g = Convert.ToDotNetRDF(TTLGraph);

            if (g != null)
                return g.Individuals();
            else
                Log.RecordError("The input string was not a correctly formed TTL graph.");

            return null;
        }

        public static List<OntologyResource> IndividualsNoOwner(this OntologyGraph ontologyGraph)
        {
            return ontologyGraph.Individuals().Where(i => !i.TriplesWithObject.Any()).ToList();
        }

        [Description("Gets all Individuals nodes from a DotNetRDF OntologyGraph object.")]
        public static List<OntologyResource> Individuals(this OntologyGraph ontologyGraph)
        {
            if (ontologyGraph == null)
                return new List<OntologyResource>();

            List<OntologyResource> individuals = ontologyGraph.AllClasses.Select(c => c.Instances.Where(i => i != null).ToList()).Where(s => s.Any()).SelectMany(l => l).ToList(); //g.Triples.Where(t => )
            Log.RecordNote("No individual found.");

            return individuals;
        }
    }
}