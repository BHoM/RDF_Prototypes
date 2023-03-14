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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapters.RDF
{
    [Description("Settings for the definition of an Ontology.")]
    public class OntologySettings : IObject
    {
        public string OntologyTitle { get; set; } = $"{DateTime.Now.ToString("yyMMdd-HHmmss")}_newBHoMOntology";
        public string OntologyDescription { get; set; } = $"New BHoM ontology";
        public string OntologyBaseAddress { get; set; } = "https://bhom.xyz/ontology";

        public TBoxSettings TBoxSettings { get; set; } = new TBoxSettings();
        public ABoxSettings ABoxSettings { get; set; } = new ABoxSettings();

        public OntologySettings()
        {
            TBoxSettings = new TBoxSettings()
            {
                CustomObjectTypesBaseAddress = OntologyBaseAddress
            };
        }
    }

    [Description("Settings for the definition of an Ontology's T-Box.")]
    public class TBoxSettings : IObject
    {
        [Description("The base address where the ontology definition for Custom Types will be hosted. Custom Types are produced when computing an ontology that includes BHoM CustomObjects.")]
        public string CustomObjectTypesBaseAddress { get; set; } = $"http://customizeMeFrom-OntologySettings.TBoxSettings.{nameof(CustomObjectTypesBaseAddress)}";

        [Description("Types found in this dictionary will use the corresponding string URI when converted to ontology.")]
        public Dictionary<Type, string> TypeUris { get; set; } = new Dictionary<Type, string>();

        [Description("Default base URI used for unknown types. The full URI for the type will be this uri plus #typeName appended at its end.")]
        public string DefaultBaseUriForUnknownTypes { get; set; } = $"http://customizeFrom-OntologySettings.TBoxSettings.{nameof(DefaultBaseUriForUnknownTypes)}";

        [Description("If true, any CustomObject that has a Type key in its CustomData dictionary will be treated as if it was an instance of a custom class," +
            "which will be called like the value stored in the Type key.")]
        public bool TreatCustomObjectsWithTypeKeyAsCustomObjectTypes { get; set; } = true;

        [Description("Key of the CustomData dictionary that will be sought in CustomObjects. If a value is found there, and if the above option is true," +
            "the value will be used as if the CustomObject was a class called with this value.")]
        public string CustomobjectsTypeKey { get; set; } = "Type";

        [Description("(defaults to false) If true, geometrical Types will be considered as Classes, and therefore Object Properties." +
            "Otherwise, geometrical types are considered as a DataType of type Base64Serialized, and the geometry is encoded as a Data Property.")]
        public bool GeometryAsOntologyClass { get; set; } = false;
    }

    [Description("Settings for the definition of an Ontology's A-Box.")]
    public class ABoxSettings : IObject
    {
        [Description("The base address where the individuals will be hosted.")]
        public string IndividualsBaseAddress { get; set; } = $"http://customizeFrom-OntologySettings.ABoxSettings.{nameof(IndividualsBaseAddress)}";

        [Description("If this is set to true, if an individual's ObjectProperty or Data property is null or an empty collection, it will still be added.")]
        public bool ConsiderNullOrEmptyPropertyValues { get; set; } = false;
    }
}