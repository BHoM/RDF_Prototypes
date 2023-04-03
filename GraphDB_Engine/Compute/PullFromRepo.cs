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

using BH.Engine.Adapters.RDF;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Shacl.Validation;

namespace BH.Engine.Adapters.GraphDB
{
    public static partial class Compute
    {

        [Description("Pulls RDF data from a GraphDB using its SPARQL API.")]
        public static SparqlResultSet PullFromRepo(string queryString = null, string serverAddress = "http://localhost:7200/", string repositoryName = "BHoMVisualization", bool run = false)
        {
            if (!run)
            {
                Log.RecordWarning("To pull data to GraphDB press the Button or switch the Toggle to true");
                return null;
            }

            if (!Uri.TryCreate(serverAddress, UriKind.Absolute, out var serverAddressUri))
            {
                Log.RecordError($"The Uri for {nameof(serverAddress)} is not valid.");
                return null;
            }

            var endpointRepoPullData = new Uri(serverAddress + "repositories/" + repositoryName);

            string stringQuery = queryString ?? "select * where {?s ?p ?o .}";


            // Check if the query is parsable
            SparqlQueryParser sparqlparser = new SparqlQueryParser();
            try
            {
                SparqlQuery sparqlQuery = sparqlparser.ParseFromString(stringQuery);
            }
            catch (RdfQueryException queryEx)
            {
                Log.RecordError($"The query was invalid. Message " + queryEx.Message, ex: queryEx);
            }

            // Create a new instance of the SparqlRemoteEndpoint
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(endpointRepoPullData)
            {
                UserAgent = "User-Agent: test-bot/0.0 (https://bhom.xyz; test.email@gmail.com) generic-library/0.0"
            };

            try
            {
                // Execute the SPARQL query and get the result
                SparqlResultSet resultSet = endpoint.QueryWithResultSet(stringQuery);
                return resultSet;
            }
            catch (Exception ex)
            {
                Log.RecordError($"Error querying GraphDB: {ex.Message}", ex: ex );
                return null;
            }
        }
    }
}
