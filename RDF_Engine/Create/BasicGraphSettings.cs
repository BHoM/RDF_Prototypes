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

using BH.oM.Adapters.RDF;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Create
    {
        [Description("Basic Settings for the definition of an Ontology.")]
        [Input("ontologyTitle", "Sets the title of the Ontology.")]
        [Input("ontologyDescription", "Sets the description of the ontology.")]
        [Input("tBoxURI", "The base address where the Ontology definition for Custom Types will be hosted. Custom Types are produced when computing an ontology that includes BHoM CustomObjects.")]
        [Input("aBoxURI", "The base address where the individuals will be hosted.")]
        [Input("serializeGeoToBase64", "(defaults to false) If true, geometrical types are considered as a DataType of type Base64Serialized, and the geometry is encoded as a Data Property." +
            "Otherwise, geometrical Types will be considered as Classes (i.e. their own type), and therefore are included as Object Properties.")]
        public static GraphSettings BasicGraphSettings(string ontologyTitle, string ontologyDescription, string tBoxURI, string aBoxURI, bool serializeGeoToBase64 = false)
        {
            return new GraphSettings()
            {
                TBoxSettings = new TBoxSettings()
                {
                    CustomObjectTypesBaseAddress = tBoxURI,
                    GeometryAsOntologyClass = !serializeGeoToBase64
                },
                ABoxSettings = new ABoxSettings()
                {
                    IndividualsBaseAddress = aBoxURI
                },
                OntologyBaseAddress = tBoxURI,
                OntologyTitle = ontologyTitle,
                OntologyDescription = ontologyDescription
            };
        }
    }
}
