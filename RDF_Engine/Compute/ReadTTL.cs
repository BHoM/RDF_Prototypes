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


using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        [MultiOutputAttribute(0,"KG","Ontology produced whatever")]
        [MultiOutputAttribute(1, "OS", "Ontology Settings used to construct this KG")]
        public static Output<List<object>, OntologySettings> ReadTTL(string TTLtext)
        {

            if (string.IsNullOrWhiteSpace(TTLtext))
            {
                return new Output<List<object>, OntologySettings>();

            }

            // ############### Ontology Settings ############### 

            OntologySettings ontologySettings = new OntologySettings();

            string ontologyTitle = Convert.GetStringBetweenCharacters(TTLtext, "dc:title ", "@en;");
            ontologySettings.OntologyTitle = ontologyTitle.Replace("\"", string.Empty);

            string ontologyDescription = Convert.GetStringBetweenCharacters(TTLtext, "dc:description ", "@en.");
            ontologySettings.OntologyDescription = ontologyDescription.Replace("\"", string.Empty);

            string ontologyBaseAdress = Convert.GetStringBetweenCharacters(TTLtext, "@base   <", "> .");
            ontologySettings.OntologyBaseAddress = ontologyBaseAdress;


            // ############### TBOX Settings ############### 

            // Custom Base Adress
            string customBaseAdress = null;
            string[] tokens = TTLtext.Split('#');
            foreach (string token in tokens)
            {
                if (token.Contains("customtype"))
                {
                    string found = Convert.GetUntilOrEmpty(token, "customtype");
                    customBaseAdress = found.Replace(" ", string.Empty);
                    break;
                }
            };

            if(!(customBaseAdress == null)) ontologySettings.TBoxSettings.CustomObjectTypesBaseAddress = customBaseAdress;

            
            // All 3 other Settings
            TBoxSettings defaultTboxSettings = new TBoxSettings();
            string tBoxSettingsSubString = Convert.GetStringBetweenCharacters(TTLtext, $"# {nameof(TBoxSettings)}", $"# {nameof(TBoxSettings)}");
            string[] tBoxlines = tBoxSettingsSubString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string treatAsCustomObjectTypes_string = Convert.SearchAndReplaceString(tBoxlines, $"# {nameof(defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)}: ").FirstOrDefault();
            if (treatAsCustomObjectTypes_string.IsNullOrEmpty() && bool.TryParse(treatAsCustomObjectTypes_string, out bool treatAsCustomObjectTypes))
                ontologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes = treatAsCustomObjectTypes;


            string customObjectsTypeKey = Convert.SearchAndReplaceString(tBoxlines, $"# {nameof(defaultTboxSettings.CustomobjectsTypeKey)}: ").FirstOrDefault();
            if (customObjectsTypeKey.IsNullOrEmpty())
                ontologySettings.TBoxSettings.CustomobjectsTypeKey = customObjectsTypeKey;

            List<string> typeUris = Convert.SearchAndReplaceString(tBoxlines, $"# {nameof(defaultTboxSettings.TypeUris)}: ");
            if (!typeUris.IsNullOrEmpty())
            {
                var type_uri_dict = new Dictionary<Type, string>();
                foreach (var typeUri in typeUris)
                {
                    var kv = typeUri.Split(';');
                    Type t = Type.GetType(kv.FirstOrDefault());
                    string uri = kv.LastOrDefault();

                    type_uri_dict[t] = uri;
                }

                ontologySettings.TBoxSettings.TypeUris = type_uri_dict;
            }



            // ############### ABOX Settings ############### 

            ABoxSettings defaultAboxSettings = new ABoxSettings();
            string aBoxSettingsSubString = Convert.GetStringBetweenCharacters(TTLtext, $"# {nameof(ABoxSettings)}", $"# {nameof(ABoxSettings)}");
            string[] aBoxlines = aBoxSettingsSubString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string individualBaseAdress = Convert.SearchAndReplaceString(aBoxlines, $"# {nameof(defaultAboxSettings.IndividualsBaseAddress)}: ").FirstOrDefault();
            if (individualBaseAdress.IsNullOrEmpty())
                ontologySettings.ABoxSettings.IndividualsBaseAddress = individualBaseAdress;

            string considerDefaultPropVal_string = Convert.SearchAndReplaceString(aBoxlines, $"# {nameof(defaultAboxSettings.ConsiderDefaultPropertyValues)}: ").FirstOrDefault();
            if (considerDefaultPropVal_string.IsNullOrEmpty() && bool.TryParse(considerDefaultPropVal_string, out bool considerDefaultPropVal))
                ontologySettings.ABoxSettings.ConsiderDefaultPropertyValues = considerDefaultPropVal;

            string considerNullPropVal_string = Convert.SearchAndReplaceString(aBoxlines, $"# {nameof(defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)}: ").FirstOrDefault();
            if (considerNullPropVal_string.IsNullOrEmpty() && bool.TryParse(considerNullPropVal_string, out bool considerNullPropVal))
                ontologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues = considerNullPropVal;




            Output<List<object>, OntologySettings> output = new Output<List<object>, OntologySettings>
            {
                Item1 = Convert.ToCSharpObjects(TTLtext),
                Item2 = ontologySettings
            };
            return output;
        }


        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static Output<List<object>, OntologySettings> ReadTTL(string TTLfilePath, bool active = false)
        {
            if (!active)
                return new Output<List<object>, OntologySettings>();

            string TTLtext = File.ReadAllText(TTLfilePath);
            Output<List<object>, OntologySettings> readTTLOutput = ReadTTL(TTLtext);

            return readTTLOutput;
        }

    }
}
